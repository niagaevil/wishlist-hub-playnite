using System.Collections.Generic;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace WishlistHub.Settings
{
    public class WishlistHubSettingsViewModel : ObservableObject, ISettings
    {
        private readonly WishlistHubPlugin _plugin;
        private WishlistHubSettings _editingClone;

        public WishlistHubSettings Settings { get; set; }

        public WishlistHubSettingsViewModel(WishlistHubPlugin plugin)
        {
            _plugin = plugin;
            var saved = plugin.LoadPluginSettings<WishlistHubSettings>();
            Settings = saved ?? new WishlistHubSettings();
        }

        public void BeginEdit()
        {
            _editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            Settings = _editingClone;
        }

        public void EndEdit()
        {
            _plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Settings.BaseUrl))
            {
                errors.Add("Base URL é obrigatória.");
            }

            return errors.Count == 0;
        }
    }
}
