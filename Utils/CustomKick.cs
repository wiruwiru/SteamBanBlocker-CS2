using CounterStrikeSharp.API;
using CounterStrikeSharp.API.ValveConstants.Protobuf;

namespace SteamBanBlocker.Utils;

public class CustomKick
{

    public static void KickPlayer(int userId, NetworkDisconnectionReason reason = NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED, int delay = 0)
    {
        var player = Utilities.GetPlayerFromUserid(userId);

        if (player == null || !player.IsValid || player.IsHLTV)
            return;

        player.CommitSuicide(true, true);

        if (delay > 0)
        {
            var timer = new CounterStrikeSharp.API.Modules.Timers.Timer(delay, () =>
            {
                if (!player.IsValid || player.IsHLTV)
                    return;

                player.Disconnect(reason);
            });
        }
        else
        {
            player.Disconnect(reason);
        }
    }

}