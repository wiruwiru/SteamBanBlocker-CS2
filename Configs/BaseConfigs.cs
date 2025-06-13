using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace SteamBanBlocker.Config
{
    public class BaseConfigs : BasePluginConfig
    {

        [JsonPropertyName("SteamWebAPI")]
        public string SteamWebAPI { get; set; } = "";

        [JsonPropertyName("BlockCommunityBanned")]
        public bool BlockCommunityBanned { get; set; } = false;

        [JsonPropertyName("BlockVACBanned")]
        public bool BlockVACBanned { get; set; } = true;

        [JsonPropertyName("BlockGameBanned")]
        public bool BlockGameBanned { get; set; } = true;

        [JsonPropertyName("EnableDebug")]
        public bool EnableDebug { get; set; } = false;

    }
}