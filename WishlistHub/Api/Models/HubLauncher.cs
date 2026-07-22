using System.ComponentModel;
using System.Runtime.Serialization;

namespace WishlistHub.Api.Models
{
    public enum HubLauncher
    {
        [EnumMember(Value = "other")]
        [Description("Other")]
        Other = 0,

        [EnumMember(Value = "steam")]
        Steam = 1,

        [EnumMember(Value = "ea")]
        EA = 2,

        [EnumMember(Value = "ubisoft")]
        Ubisoft = 3,

        [EnumMember(Value = "gog")]
        GOG = 4,

        [EnumMember(Value = "epic")]
        Epic = 5,

        [EnumMember(Value = "microsoft")]
        Microsoft = 6,

        [EnumMember(Value = "battle-net")]
        BattleNet = 7,

        [EnumMember(Value = "rockstar")]
        Rockstar = 8,

        [EnumMember(Value = "prime-gaming")]
        PrimeGaming = 9,

        [EnumMember(Value = "playstation")]
        Playstation = 10,

        [EnumMember(Value = "nintendo")]
        Nintendo = 11,

        [EnumMember(Value = "itch")]
        Itch = 12,

        [EnumMember(Value = "drm-free")]
        DrmFree = 13,
    }
}
