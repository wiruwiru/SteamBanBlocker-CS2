using SteamBanBlocker.Config;

namespace SteamBanBlocker.Utils;

public class Debug
{
    public static BaseConfigs? Config { get; set; }

    public static void DebugInfo(string category, string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ SteamBanBlocker - {category} ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=============================================================================");
    }

}