using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using WishlistHub.Api.Models;
using WishlistHub.Api.Services;
using WishlistHub.Settings;

namespace WishlistHub
{
    public class WishlistHubPlugin : GenericPlugin
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private WishlistHubSettingsViewModel _settings;

        public override Guid Id { get; } = Guid.Parse("b7e4c2a1-9f3d-4e8b-a6c5-1d2e3f4a5b6c");

        public WishlistHubPlugin(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties { HasSettings = true };
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return _settings ?? (_settings = new WishlistHubSettingsViewModel(this));
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            // UI XAML pode ser adicionada depois; settings editáveis via API / JSON por enquanto.
            return null;
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            yield return new MainMenuItem
            {
                Description = "Enviar jogos ao Wishlist Hub",
                MenuSection = "@Wishlist Hub",
                Action = _ => _ = SendGamesAsync(),
            };
        }

        private async Task SendGamesAsync()
        {
            var settingsVm = GetSettings(false) as WishlistHubSettingsViewModel;
            var settings = settingsVm?.Settings;
            if (settings == null || string.IsNullOrWhiteSpace(settings.AuthenticationToken))
            {
                PlayniteApi.Dialogs.ShowErrorMessage(
                    "Configure o token em Add-ons → Extension settings → Wishlist Hub.",
                    "Wishlist Hub");
                return;
            }

            var map = new LibraryToHubLauncherMap();
            var games = PlayniteApi.Database.Games
                .Where(g => !g.Hidden)
                .Select(g =>
                {
                    var launcher = map.GetLauncher(g.PluginId);
                    return new { Game = g, Launcher = launcher };
                })
                .Where(x => settings.IsLauncherEnabled(x.Launcher))
                .Select(x => GameWithLauncher.FromGame(x.Game, x.Launcher))
                .ToList();

            if (games.Count == 0)
            {
                PlayniteApi.Dialogs.ShowMessage("Nenhum jogo nas bibliotecas selecionadas.", "Wishlist Hub");
                return;
            }

            try
            {
                using (var client = new WishlistHubApiClient(settings))
                using (var cts = new CancellationTokenSource())
                {
                    var result = await client.ImportGamesAsync(games, cts.Token).ConfigureAwait(true);
                    var results = result?.Data?.Result ?? new List<ImportResult>();
                    var added = results.Count(r => string.Equals(r.Status, "Added", StringComparison.OrdinalIgnoreCase));
                    var skipped = results.Count(r => string.Equals(r.Status, "Skipped", StringComparison.OrdinalIgnoreCase));
                    var miss = results.Count(r => string.Equals(r.Status, "Miss", StringComparison.OrdinalIgnoreCase));
                    var ignored = results.Count(r => string.Equals(r.Status, "Ignored", StringComparison.OrdinalIgnoreCase));
                    var errors = results.Count(r => string.Equals(r.Status, "Error", StringComparison.OrdinalIgnoreCase));
                    PlayniteApi.Dialogs.ShowMessage(
                        $"Enviado: {results.Count} jogos.\nAdded: {added}\nSkipped: {skipped}\nMiss: {miss}\nIgnored: {ignored}\nError: {errors}",
                        "Wishlist Hub");
                }
            }
            catch (UnauthorizedAccessException)
            {
                PlayniteApi.Dialogs.ShowErrorMessage(
                    "Token inválido. Gere um novo em Ferramentas → Playnite no Wishlist Hub.",
                    "Wishlist Hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Falha ao enviar jogos ao Wishlist Hub");
                PlayniteApi.Dialogs.ShowErrorMessage(ex.Message, "Wishlist Hub");
            }
        }
    }
}
