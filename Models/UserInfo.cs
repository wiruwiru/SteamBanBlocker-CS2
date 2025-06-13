namespace SteamBanBlocker.Models
{
    public class SteamUserInfo
    {
        public string SteamId { get; set; } = string.Empty;
        public bool IsCommunityBanned { get; set; }
        public bool IsVACBanned { get; set; }
        public bool IsGameBanned { get; set; }

    }
}