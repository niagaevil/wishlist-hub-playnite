using System.Collections.Generic;
using Playnite.SDK.Data;

namespace WishlistHub.Settings
{
    public class WishlistHubSettings : ObservableObject
    {
        private string _authenticationToken = string.Empty;
        private string _baseUrl = "https://wishlist-hub.paz.poa.br";
        private bool _syncNewlyAddedGames = true;
        private bool _useStatusTags = true;
        private bool _showNotifications = true;
        private bool _includeSteam = true;
        private bool _includeEpic = true;
        private bool _includeGog = true;
        private bool _includeUbisoft = true;
        private bool _includeAmazon = true;
        private bool _includeEa = true;
        private bool _includeBattleNet = true;
        private bool _includeMicrosoft = true;
        private bool _includeRockstar = true;
        private bool _includeItch = true;
        private bool _includeOther = true;

        public string AuthenticationToken
        {
            get => _authenticationToken;
            set => SetValue(ref _authenticationToken, (value ?? string.Empty).Trim());
        }

        public string BaseUrl
        {
            get => _baseUrl;
            set => SetValue(ref _baseUrl, string.IsNullOrWhiteSpace(value)
                ? "https://wishlist-hub.paz.poa.br"
                : value.Trim().TrimEnd('/'));
        }

        /// <summary>Envia jogos novos ao Hub após adicionar na biblioteca do Playnite.</summary>
        public bool SyncNewlyAddedGames
        {
            get => _syncNewlyAddedGames;
            set => SetValue(ref _syncNewlyAddedGames, value);
        }

        public bool UseStatusTags
        {
            get => _useStatusTags;
            set => SetValue(ref _useStatusTags, value);
        }

        public bool ShowNotifications
        {
            get => _showNotifications;
            set => SetValue(ref _showNotifications, value);
        }

        public bool IncludeSteam { get => _includeSteam; set => SetValue(ref _includeSteam, value); }
        public bool IncludeEpic { get => _includeEpic; set => SetValue(ref _includeEpic, value); }
        public bool IncludeGog { get => _includeGog; set => SetValue(ref _includeGog, value); }
        public bool IncludeUbisoft { get => _includeUbisoft; set => SetValue(ref _includeUbisoft, value); }
        public bool IncludeAmazon { get => _includeAmazon; set => SetValue(ref _includeAmazon, value); }
        public bool IncludeEa { get => _includeEa; set => SetValue(ref _includeEa, value); }
        public bool IncludeBattleNet { get => _includeBattleNet; set => SetValue(ref _includeBattleNet, value); }
        public bool IncludeMicrosoft { get => _includeMicrosoft; set => SetValue(ref _includeMicrosoft, value); }
        public bool IncludeRockstar { get => _includeRockstar; set => SetValue(ref _includeRockstar, value); }
        public bool IncludeItch { get => _includeItch; set => SetValue(ref _includeItch, value); }
        public bool IncludeOther { get => _includeOther; set => SetValue(ref _includeOther, value); }

        public bool IsLauncherEnabled(Api.Models.HubLauncher launcher)
        {
            switch (launcher)
            {
                case Api.Models.HubLauncher.Steam: return IncludeSteam;
                case Api.Models.HubLauncher.Epic: return IncludeEpic;
                case Api.Models.HubLauncher.GOG: return IncludeGog;
                case Api.Models.HubLauncher.Ubisoft: return IncludeUbisoft;
                case Api.Models.HubLauncher.PrimeGaming: return IncludeAmazon;
                case Api.Models.HubLauncher.EA: return IncludeEa;
                case Api.Models.HubLauncher.BattleNet: return IncludeBattleNet;
                case Api.Models.HubLauncher.Microsoft: return IncludeMicrosoft;
                case Api.Models.HubLauncher.Rockstar: return IncludeRockstar;
                case Api.Models.HubLauncher.Itch: return IncludeItch;
                default: return IncludeOther;
            }
        }
    }
}
