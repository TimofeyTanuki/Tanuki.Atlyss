using BepInEx.Configuration;
using Tanuki.Atlyss.Core.Types.Settings;

namespace Tanuki.Atlyss.Core.Types.Configuration;

internal sealed class Network(ConfigFile configFile)
{
    private const string SECTION_NAME = "Network";

    public ConfigEntry<ENetworkRateLimiter> RateLimiter =
        configFile.Bind(
            SECTION_NAME,
            "RateLimiter",
            ENetworkRateLimiter.Window,
            "Skips rate limiter configuration, disables rate limiting, or enables a window-based rate limiter."
        );

    public ConfigEntry<ushort> SteamNetworkMessagePollerBuffer =
        configFile.Bind(
            SECTION_NAME,
            "SteamNetworkMessagePollerBuffer",
            (ushort)32,
            new ConfigDescription(
                "The size of the Steam network message puller buffer.",
                new AcceptableValueRange<ushort>(8, ushort.MaxValue)
            )
        );

    public ConfigEntry<bool> PreventLobbyOwnerRateLimiting =
        configFile.Bind(
            SECTION_NAME,
            "PreventLobbyOwnerRateLimiting",
            true,
            "Prevents the lobby owner from being rate limited."
        );

    public ConfigEntry<uint> WindowRateLimiter_Bandwidth =
        configFile.Bind(
            SECTION_NAME,
            "WindowRateLimiter_Bandwidth",
            256000U,
            new ConfigDescription(
                "The maximum amount of data in bytes per window, upon reaching which the connection with the client will be terminated until the next window.",
                new AcceptableValueRange<uint>(1024U, uint.MaxValue)
            )
        );

    public ConfigEntry<float> WindowRateLimiter_Window =
        configFile.Bind(
            SECTION_NAME,
            "WindowRateLimiter_Window",
            1f,
            new ConfigDescription(
                "The size of the rate limiter window in seconds.",
                new AcceptableValueRange<float>(0, float.MaxValue)
            )
        );
}
