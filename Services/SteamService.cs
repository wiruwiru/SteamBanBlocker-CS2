using Newtonsoft.Json.Linq;

using SteamBanBlocker.Config;
using SteamBanBlocker.Models;
using SteamBanBlocker.Utils;

namespace SteamBanBlocker.Services
{
	public class SteamService
	{
		private readonly BaseConfigs _config;
		private readonly string _steamWebAPIKey;
		private readonly HttpClient _httpClient;
		public SteamUserInfo? UserInfo = null;

		public SteamService(SteamRestrictPlugin plugin, SteamUserInfo userInfo)
		{
			_httpClient = plugin.Client;
			_config = plugin.Config;
			_steamWebAPIKey = _config.SteamWebAPI;
			UserInfo = userInfo;
		}

		public async Task FetchSteamUserInfo(string steamId)
		{
			if (UserInfo != null)
			{
				await FetchPlayerBansAsync(steamId, UserInfo);
			}
		}

		private async Task FetchPlayerBansAsync(string steamId, SteamUserInfo userInfo)
		{
			var url = $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={_steamWebAPIKey}&steamids={steamId}";
			var json = await GetApiResponseAsync(url);
			if (json != null)
			{
				ParsePlayerBans(json, userInfo);
			}
		}

		private async Task<string?> GetApiResponseAsync(string url)
		{
			try
			{
				var response = await _httpClient.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsStringAsync();
				}
			}
			catch (Exception e)
			{
				Debug.DebugInfo("SteamAPI", $"An error occurred while fetching API response: {e.Message}");
			}
			return null;
		}

		private void ParsePlayerBans(string json, SteamUserInfo userInfo)
		{
			try
			{
				JObject data = JObject.Parse(json);
				JToken? playerBan = data["players"]?.FirstOrDefault();

				if (playerBan != null)
				{
					userInfo.IsCommunityBanned = (bool)(playerBan["CommunityBanned"] ?? false);
					userInfo.IsVACBanned = (bool)(playerBan["VACBanned"] ?? false);

					int numberOfGameBans = (int)(playerBan["NumberOfGameBans"] ?? 0);
					userInfo.IsGameBanned = numberOfGameBans > 0;

					Debug.DebugInfo("SteamAPI", $"Player {userInfo.SteamId} - Community Banned: {userInfo.IsCommunityBanned}, VAC Banned: {userInfo.IsVACBanned}, Game Banned: {userInfo.IsGameBanned} (Game Bans: {numberOfGameBans})");
				}
			}
			catch (Exception e)
			{
				Debug.DebugInfo("SteamAPI", $"Error parsing player bans: {e.Message}");
			}
		}
	}
}