using System;
using System.Collections.Generic;
using WishlistHub.Api.Models;

namespace WishlistHub.Api.Services
{
    public class LibraryToHubLauncherMap
    {
        private readonly Dictionary<Guid, HubLauncher> _map = new Dictionary<Guid, HubLauncher>
        {
            { Guid.Parse("CB91DFC9-B977-43BF-8E70-55F46E410FAB"), HubLauncher.Steam },
            { Guid.Parse("85DD7072-2F20-4E76-A007-41035E390724"), HubLauncher.EA },
            { Guid.Parse("C2F038E5-8B92-4877-91F1-DA9094155FC5"), HubLauncher.Ubisoft },
            { Guid.Parse("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E"), HubLauncher.GOG },
            { Guid.Parse("03689811-3F33-4DFB-A121-2EE168FB9A5C"), HubLauncher.GOG },
            { Guid.Parse("00000002-DBD1-46C6-B5D0-B1BA559D10E4"), HubLauncher.Epic },
            { Guid.Parse("EAD65C3B-2F8F-4E37-B4E6-B3DE6BE540C6"), HubLauncher.Epic },
            { Guid.Parse("7e4fbb5e-2ae3-48d4-8ba0-6b30e7a4e287"), HubLauncher.Microsoft },
            { Guid.Parse("E3C26A3D-D695-4CB7-A769-5FF7612C7EDD"), HubLauncher.BattleNet },
            { Guid.Parse("88409022-088a-4de8-805a-fdbac291f00a"), HubLauncher.Rockstar },
            { Guid.Parse("402674cd-4af6-4886-b6ec-0e695bfa0688"), HubLauncher.PrimeGaming },
            { Guid.Parse("5901B4B4-774D-411A-9CCE-807C5CA49D88"), HubLauncher.PrimeGaming },
            { Guid.Parse("e4ac81cb-1b1a-4ec9-8639-9a9633989a71"), HubLauncher.Playstation },
            { Guid.Parse("e4ac81cb-1b1a-4ec9-8639-9a9633989a72"), HubLauncher.Nintendo },
            { Guid.Parse("00000001-EBB2-4EEC-ABCB-7C89937A42BB"), HubLauncher.Itch },
        };

        public HubLauncher GetLauncher(Guid pluginId)
        {
            return _map.TryGetValue(pluginId, out var launcher) ? launcher : HubLauncher.Other;
        }
    }
}
