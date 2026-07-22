using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Playnite.SDK.Models;

namespace WishlistHub.Api.Models
{
    public class GameWithLauncher
    {
        public static GameWithLauncher FromGame(Game game, HubLauncher launcher)
        {
            return new GameWithLauncher
            {
                GGLauncher = launcher,
                Id = game.Id,
                GameId = game.GameId,
                Links = game.Links,
                Source = game.Source,
                ReleaseDate = game.ReleaseDate,
                ReleaseYear = game.ReleaseYear,
                Name = game.Name,
            };
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("gg_launcher")]
        public HubLauncher GGLauncher { get; set; }

        public Guid Id { get; set; }

        public string GameId { get; set; }

        public IEnumerable<Link> Links { get; set; }

        public GameSource Source { get; set; }

        public ReleaseDate? ReleaseDate { get; set; }

        public int? ReleaseYear { get; set; }

        public string Name { get; set; }
    }
}
