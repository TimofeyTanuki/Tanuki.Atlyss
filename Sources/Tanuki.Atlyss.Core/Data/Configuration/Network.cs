using BepInEx.Configuration;

namespace Tanuki.Atlyss.Core.Data.Configuration;

internal sealed class Network(ConfigFile configFile)
{
    private const string SECTION_NAME = "NetworkSection";

    public ConfigEntry<int> MainSteamMessageChannel =
        configFile.Bind(
            SECTION_NAME,
            "MainSteamMessageChannel",
            13052003,
            "Main Steam message channel. It is not recommended to change this because all clients should listen to the same channel."
        );

    public ConfigEntry<uint> RateLimiterBandwidth =
        configFile.Bind(
            SECTION_NAME,
            "RateLimiterBandwidth",
            256000U,
            "The maximum amount of data in bytes per window, upon reaching which the connection with the client will be terminated until the next window."
        );

    public ConfigEntry<float> RateLimiterWindow =
        configFile.Bind(
            SECTION_NAME,
            "RateLimiterWindow",
            1f,
            "The size of the rate limiter window in seconds."
        );

    public ConfigEntry<bool> PreventLobbyOwnerRateLimiting =
        configFile.Bind(
            SECTION_NAME,
            "PreventLobbyOwnerRateLimiting",
            true,
            "Prevents the lobby owner from being rate limited."
        );

    public ConfigEntry<ushort> SteamNetworkMessagePollerBuffer =
        configFile.Bind(
            SECTION_NAME,
            "SteamNetworkMessagePollerBuffer",
            (ushort)16,
            "The size of the Steam network message puller buffer."
        );
}
