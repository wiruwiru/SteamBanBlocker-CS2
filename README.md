# SteamBanBlocker CS2
Automatic ban detection and blocking system for Counter-Strike 2 servers that prevents players with VAC, Game, or Community bans from connecting to your server.

## 🚀 Installation
1. Install [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod](https://www.sourcemm.net/downloads.php/?branch=master)
2. Download [SteamBanBlocker](https://github.com/yourrepo/SteamBanBlocker-CS2/releases/latest) from releases
3. Extract and upload to your game server at `csgo/addons/counterstrikesharp/plugins/SteamBanBlocker`
4. Start server and configure the generated config file located at `csgo/addons/counterstrikesharp/configs/plugins/SteamBanBlocker`

---

## 📋 Main Configuration Parameters

| Parameter | Description | Default | Required |
|-----------|-------------|---------|----------|
| `SteamWebAPI` | Steam Web API key from https://steamcommunity.com/dev/apikey | `""` | **YES** |
| `BlockCommunityBanned` | Block players with Steam Community bans | `false` | **YES** |
| `BlockVACBanned` | Block players with VAC (Valve Anti-Cheat) bans | `true` | **YES** |
| `BlockGameBanned` | Block players with Game bans | `true` | **YES** |
| `EnableDebug` | Enable debug console output | `false` | **YES** |

### Configuration File Structure

```json
{
  "SteamWebAPI": "YOUR_STEAM_WEB_API_KEY_HERE",
  "BlockCommunityBanned": false,
  "BlockVACBanned": true,
  "BlockGameBanned": true,
  "EnableDebug": false
}
```

---

## 🔑 Steam Web API Setup

1. Visit [Steam Web API Key Registration](https://steamcommunity.com/dev/apikey)
2. Enter your domain name (can be localhost for local servers)
3. Agree to the terms and generate your API key
4. Copy the generated key and paste it in the `SteamWebAPI` field in your config

**⚠️ Important**: The plugin will not work without a valid Steam Web API key!

---

## 🛡️ Ban Types Explained

### VAC Bans (Valve Anti-Cheat)
- Automatically issued by Valve's anti-cheat system
- Indicates the player was caught cheating in VAC-secured games
- **Recommended**: Keep `BlockVACBanned` enabled

### Game Bans
- Issued by game developers for various violations
- Can include cheating, griefing, or other rule violations
- **Recommended**: Keep `BlockGameBanned` enabled

### Community Bans
- Issued by Steam for community guideline violations
- Includes scamming, harassment, or inappropriate content
- **Optional**: Enable `BlockCommunityBanned` if desired

---

## 📊 Support

For issues, questions, or feature requests, please visit our [GitHub Issues](https://github.com/yourrepo/SteamBanBlocker-CS2/issues) page.