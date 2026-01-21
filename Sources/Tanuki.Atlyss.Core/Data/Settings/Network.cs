namespace Tanuki.Atlyss.Core.Data.Settings;

public sealed class Network
{
    internal int mainSteamMessageChannel;
    internal uint rateLimiterBandwidth;
    internal float rateLimiterWindow;
    internal bool preventLobbyOwnerRateLimiting;
    internal ushort steamNetworkMessagePollerBuffer;

    public int MainSteamMessageChannel => mainSteamMessageChannel;
    public uint RateLimiterBandwidth => rateLimiterBandwidth;
    public float RateLimiterWindow => rateLimiterWindow;
    public bool PreventLobbyOwnerRateLimiting => preventLobbyOwnerRateLimiting;
    public ushort SteamNetworkMessagePollerBuffer => steamNetworkMessagePollerBuffer;
}
