using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace WishlistHub.Services
{
    public static class WishlistHubTags
    {
        public const string Prefix = "[WishlistHub] ";
        public const string Synced = Prefix + "Synced";
        public const string Miss = Prefix + "Miss";
        public const string Ignored = Prefix + "Ignored";
        public const string Error = Prefix + "Error";

        private static readonly string[] All =
        {
            Synced, Miss, Ignored, Error,
        };

        public static void ApplyStatus(IPlayniteAPI api, Game game, string status, bool enabled)
        {
            if (!enabled || game == null)
            {
                return;
            }

            var tagName = MapStatus(status);
            if (tagName == null)
            {
                return;
            }

            ClearWishlistHubTags(api, game);
            var tag = api.Database.Tags.Add(tagName);
            if (game.TagIds == null)
            {
                game.TagIds = new List<Guid>();
            }

            if (!game.TagIds.Contains(tag.Id))
            {
                game.TagIds.Add(tag.Id);
            }

            api.Database.Games.Update(game);
        }

        public static void ClearWishlistHubTags(IPlayniteAPI api, Game game)
        {
            if (game?.TagIds == null || game.TagIds.Count == 0)
            {
                return;
            }

            var remove = new List<Guid>();
            foreach (var id in game.TagIds)
            {
                var tag = api.Database.Tags.Get(id);
                if (tag != null && All.Contains(tag.Name))
                {
                    remove.Add(id);
                }
            }

            foreach (var id in remove)
            {
                game.TagIds.Remove(id);
            }
        }

        private static string MapStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return null;
            }

            switch (status.Trim().ToLowerInvariant())
            {
                case "added":
                case "skipped":
                    return Synced;
                case "miss":
                    return Miss;
                case "ignored":
                    return Ignored;
                case "error":
                    return Error;
                default:
                    return null;
            }
        }
    }
}
