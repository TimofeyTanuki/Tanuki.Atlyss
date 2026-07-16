namespace Tanuki.Atlyss.Network.Types.Tanuki;

public sealed class Services
{
    internal Network.Services.PacketProcessor packetProcessor = null!;
    internal IRateLimiter? rateLimiter;

    public Network.Services.PacketProcessor PacketProcessor => packetProcessor;
    public IRateLimiter? RateLimiter
    {
        get => rateLimiter;
        set => rateLimiter = value;
    }

    internal Services() { }
}
