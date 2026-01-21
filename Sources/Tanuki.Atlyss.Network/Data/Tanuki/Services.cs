namespace Tanuki.Atlyss.Network.Data.Tanuki;

public sealed class Services
{
    internal Network.Services.PacketProcessor packetProcessor = null!;
    internal Network.Services.RateLimiter rateLimiter = null!;

    public Network.Services.PacketProcessor PacketProcessor => packetProcessor;
    internal Network.Services.RateLimiter RateLimiter => rateLimiter;

    internal Services() { }
}
