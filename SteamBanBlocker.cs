using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.ValveConstants.Protobuf;

using SteamBanBlocker.Config;
using SteamBanBlocker.Models;
using SteamBanBlocker.Utils;
using SteamBanBlocker.Services;

namespace SteamBanBlocker;

[MinimumApiVersion(300)]
public class SteamRestrictPlugin : BasePlugin, IPluginConfig<BaseConfigs>
{
    public override string ModuleName => "SteamBanBlocker";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "luca.uy";
    public override string ModuleDescription => "Blocks players with VAC, Game, or Community bans from connecting to your server.";

    public readonly HttpClient Client = new HttpClient();
    private bool g_bSteamAPIActivated = false;
    private CounterStrikeSharp.API.Modules.Timers.Timer?[] g_hTimer = new CounterStrikeSharp.API.Modules.Timers.Timer?[65];

    public required BaseConfigs Config { get; set; }
    public void OnConfigParsed(BaseConfigs config)
    {

        if (string.IsNullOrEmpty(config.SteamWebAPI))
            Debug.DebugInfo("Config", "This plugin won't work because Web API is empty.");

        Config = config;
        Debug.Config = Config;
    }

    public override void Load(bool hotReload)
    {

        RegisterListener<Listeners.OnGameServerSteamAPIActivated>(() => { g_bSteamAPIActivated = true; });
        RegisterListener<Listeners.OnClientConnect>((int slot, string name, string ipAddress) => { g_hTimer[slot]?.Kill(); });
        RegisterListener<Listeners.OnClientDisconnect>((int slot) => { g_hTimer[slot]?.Kill(); });
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull, HookMode.Post);

        if (hotReload)
        {
            g_bSteamAPIActivated = true;
            foreach (var player in Utilities.GetPlayers().Where(m => m.Connected == PlayerConnectedState.PlayerConnected && !m.IsHLTV && !m.IsBot && m.SteamID.ToString().Length == 17))
            {
                OnPlayerConnectFull(player);
            }
        }
    }

    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        if (player == null)
            return HookResult.Continue;

        OnPlayerConnectFull(player);
        return HookResult.Continue;
    }

    private void OnPlayerConnectFull(CCSPlayerController player)
    {
        if (string.IsNullOrEmpty(Config.SteamWebAPI))
            return;

        if (player.IsBot || player.IsHLTV)
            return;

        if (player.AuthorizedSteamID == null)
        {
            g_hTimer[player.Slot] = AddTimer(1.0f, () =>
            {
                if (player.AuthorizedSteamID != null)
                {
                    g_hTimer[player.Slot]?.Kill();
                    OnPlayerConnectFull(player);
                    return;
                }
            }, TimerFlags.REPEAT);
            return;
        }

        if (!g_bSteamAPIActivated)
            return;

        ulong authorizedSteamID = player.AuthorizedSteamID.SteamId64;
        nint handle = player.Handle;

        Task.Run(() =>
        {
            Server.NextWorldUpdate(() =>
            {
                CheckUserViolations(handle, authorizedSteamID);
            });
        });
    }

    private void CheckUserViolations(nint handle, ulong authorizedSteamID)
    {
        var userInfo = new SteamUserInfo { SteamId = authorizedSteamID.ToString() };
        SteamService steamService = new SteamService(this, userInfo);

        Task.Run(async () =>
        {
            await steamService.FetchSteamUserInfo(authorizedSteamID.ToString());

            SteamUserInfo? fetchedUserInfo = steamService.UserInfo;

            Server.NextWorldUpdate(() =>
            {
                CCSPlayerController? player = Utilities.GetPlayerFromSteamId(authorizedSteamID);

                if (player?.IsValid == true && fetchedUserInfo != null)
                {
                    if (IsRestrictionViolated(player, fetchedUserInfo))
                    {
                        var reason = GetKickReason(fetchedUserInfo);
                        CustomKick.KickPlayer(player.UserId ?? 0, reason);

                        Server.PrintToChatAll($"{Localizer["prefix"]} {Localizer["kicked", player.PlayerName]}");
                    }
                }
            });
        });
    }

    private bool IsRestrictionViolated(CCSPlayerController player, SteamUserInfo userInfo)
    {

        if (Config.BlockCommunityBanned && userInfo.IsCommunityBanned)
            return true;

        if (Config.BlockGameBanned && userInfo.IsGameBanned)
            return true;

        if (Config.BlockVACBanned && userInfo.IsVACBanned)
            return true;

        return false;
    }

    private NetworkDisconnectionReason GetKickReason(SteamUserInfo userInfo)
    {
        if (userInfo.IsVACBanned && Config.BlockVACBanned)
            return NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED_VACNETABNORMALBEHAVIOR;

        if (userInfo.IsGameBanned && Config.BlockGameBanned)
            return NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED_VACNETABNORMALBEHAVIOR;

        if (userInfo.IsCommunityBanned && Config.BlockCommunityBanned)
            return NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED_VACNETABNORMALBEHAVIOR;

        return NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED;
    }

}