namespace Tanuki.Atlyss.Network.Data.Packets;

public sealed class RateLimitEntry
{
    public float NextRefresh = 0;
    public uint Usage = 0;
}
