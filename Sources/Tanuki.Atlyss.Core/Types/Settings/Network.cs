namespace Tanuki.Atlyss.Core.Types.Settings;

public sealed class Network
{
    internal ushort steamNetworkMessagePollerBuffer;
    internal ENetworkRateLimiter rateLimiter;
    internal bool preventLobbyOwnerRateLimiting;
    internal NetworkWindowRateLimiter windowRateLimiter;

    public ushort SteamNetworkMessagePollerBuffer => steamNetworkMessagePollerBuffer;
    public ENetworkRateLimiter RateLimiter => rateLimiter;
    public bool PreventLobbyOwnerRateLimiting => preventLobbyOwnerRateLimiting;
    public NetworkWindowRateLimiter WindowRateLimiter => windowRateLimiter;

    internal Network()
    {
        rateLimiter = ENetworkRateLimiter.Disabled;
        windowRateLimiter = new();
    }
}
