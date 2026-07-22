using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using WishlistHub.Api.Models;
using WishlistHub.Api.Services;
using WishlistHub.Queue;
using WishlistHub.Services;
using WishlistHub.Settings;

namespace WishlistHub
{
    public class WishlistHubPlugin : GenericPlugin
    {
        public const string ExtensionId = "WishlistHub_Playnite";
        private static readonly ILogger Logger = LogManager.GetLogger();

        private WishlistHubSettingsViewModel _settings;
        private ImportQueue _queue;
        private readonly LibraryToHubLauncherMap _launcherMap = new LibraryToHubLauncherMap();
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public override Guid Id { get; } = Guid.Parse("b7e4c2a1-9f3d-4e8b-a6c5-1d2e3f4a5b6c");

        public WishlistHubPlugin(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties { HasSettings = true };

            _queue = new ImportQueue(
                Path.Combine(GetPluginUserDataPath(), "queue.json"),
                ProcessQueuedGamesAsync);

            PlayniteApi.Database.Games.ItemCollectionChanged += (_, args) =>
            {
                var settings = LoadPluginSettings<WishlistHubSettings>() ?? new WishlistHubSettings();
                if (!settings.SyncNewlyAddedGames || string.IsNullOrWhiteSpace(settings.AuthenticationToken))
                {
                    return;
                }

                var ids = args.AddedItems?.Select(g => g.Id).ToList();
                if (ids == null || ids.Count == 0)
                {
                    return;
                }

                _queue.Enqueue(ids);
            };
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            _queue?.ProcessInBackground();
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            _queue?.ProcessInBackground();
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return _settings ?? (_settings = new WishlistHubSettingsViewModel(this));
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new WishlistHubSettingsView();
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            yield return new MainMenuItem
            {
                Description = "Enviar jogos ao Wishlist Hub",
                MenuSection = "@Wishlist Hub",
                Action = a => { _ = SendSelectedOrAllAsync(null, showDialog: true); },
            };
            yield return new MainMenuItem
            {
                Description = "Processar fila automática agora",
                MenuSection = "@Wishlist Hub",
                Action = a => _queue.ProcessInBackground(),
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            yield return new GameMenuItem
            {
                Description = "Enviar ao Wishlist Hub",
                MenuSection = "Wishlist Hub",
                Action = a => { _ = SendSelectedOrAllAsync(a.Games, showDialog: true); },
            };
        }

        private async Task ProcessQueuedGamesAsync(IReadOnlyCollection<Guid> gameIds)
        {
            var games = gameIds
                .Select(id => PlayniteApi.Database.Games.Get(id))
                .Where(g => g != null && !g.Hidden)
                .Cast<Game>()
                .ToList();
            if (games.Count == 0)
            {
                return;
            }

            await SendSelectedOrAllAsync(games, showDialog: false).ConfigureAwait(false);
        }

        private async Task SendSelectedOrAllAsync(IList<Game> selected, bool showDialog)
        {
            var settingsVm = _settings ?? new WishlistHubSettingsViewModel(this);
            _settings = settingsVm;
            var settings = settingsVm.Settings;
            if (settings == null || string.IsNullOrWhiteSpace(settings.AuthenticationToken))
            {
                if (showDialog)
                {
                    PlayniteApi.Dialogs.ShowErrorMessage(
                        "Configure o token em Add-ons → Extension settings → Wishlist Hub.",
                        "Wishlist Hub");
                }
                else
                {
                    Logger.Warn("WishlistHub auto-sync skipped: missing token");
                }

                return;
            }

            List<Game> source;
            if (selected != null && selected.Count > 0)
            {
                source = selected.Where(g => g != null && !g.Hidden).ToList();
            }
            else
            {
                source = PlayniteApi.Database.Games.Where(g => !g.Hidden).ToList();
            }

            var payload = new List<(Game Game, GameWithLauncher Dto)>();
            foreach (var game in source)
            {
                var launcher = _launcherMap.GetLauncher(game.PluginId);
                if (!settings.IsLauncherEnabled(launcher))
                {
                    continue;
                }

                payload.Add((game, GameWithLauncher.FromGame(game, launcher)));
            }

            if (payload.Count == 0)
            {
                if (showDialog)
                {
                    PlayniteApi.Dialogs.ShowMessage("Nenhum jogo nas bibliotecas selecionadas.", "Wishlist Hub");
                }

                return;
            }

            if (!await _sendLock.WaitAsync(0).ConfigureAwait(false))
            {
                if (showDialog)
                {
                    PlayniteApi.Dialogs.ShowMessage("Já há um envio em andamento.", "Wishlist Hub");
                }

                return;
            }

            try
            {
                using (var client = new WishlistHubApiClient(settings))
                using (var cts = new CancellationTokenSource())
                {
                    var result = await client
                        .ImportGamesAsync(payload.Select(p => p.Dto).ToList(), cts.Token)
                        .ConfigureAwait(true);
                    var results = result?.Data?.Result ?? new List<ImportResult>();
                    ApplyTags(settings, payload, results);

                    var added = results.Count(r => Eq(r.Status, "Added"));
                    var skipped = results.Count(r => Eq(r.Status, "Skipped"));
                    var miss = results.Count(r => Eq(r.Status, "Miss"));
                    var ignored = results.Count(r => Eq(r.Status, "Ignored"));
                    var errors = results.Count(r => Eq(r.Status, "Error"));
                    var summary =
                        $"Enviado: {results.Count}\nAdded: {added}\nSkipped: {skipped}\nMiss: {miss}\nIgnored: {ignored}\nError: {errors}";

                    if (showDialog)
                    {
                        PlayniteApi.Dialogs.ShowMessage(summary, "Wishlist Hub");
                    }
                    else if (settings.ShowNotifications)
                    {
                        PlayniteApi.Notifications.Add(new NotificationMessage(
                            "wishlist-hub-sync",
                            "Wishlist Hub: " + summary.Replace("\n", " · "),
                            NotificationType.Info));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (showDialog)
                {
                    PlayniteApi.Dialogs.ShowErrorMessage(
                        "Token inválido. Gere um novo em Ferramentas → Playnite no Wishlist Hub.",
                        "Wishlist Hub");
                }
                else if (settings.ShowNotifications)
                {
                    PlayniteApi.Notifications.Add(new NotificationMessage(
                        "wishlist-hub-auth",
                        "Wishlist Hub: token inválido — regenere em Ferramentas → Playnite.",
                        NotificationType.Error));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Falha ao enviar jogos ao Wishlist Hub");
                if (showDialog)
                {
                    PlayniteApi.Dialogs.ShowErrorMessage(ex.Message, "Wishlist Hub");
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private void ApplyTags(
            WishlistHubSettings settings,
            List<(Game Game, GameWithLauncher Dto)> payload,
            List<ImportResult> results)
        {
            if (!settings.UseStatusTags || results == null)
            {
                return;
            }

            var byId = results
                .Where(r => r.Id != Guid.Empty)
                .GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.Last().Status);

            foreach (var item in payload)
            {
                if (byId.TryGetValue(item.Game.Id, out var status))
                {
                    WishlistHubTags.ApplyStatus(PlayniteApi, item.Game, status, true);
                }
            }
        }

        private static bool Eq(string a, string b) =>
            string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
