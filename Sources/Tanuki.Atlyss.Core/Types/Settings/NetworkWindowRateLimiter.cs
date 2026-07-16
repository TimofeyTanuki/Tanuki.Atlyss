namespace Tanuki.Atlyss.Core.Types.Settings;

public sealed class NetworkWindowRateLimiter
{
    internal uint bandwidth;
    internal float window;

    public uint Bandwidth => bandwidth;
    public float Window => window;

    internal NetworkWindowRateLimiter()
    {
        bandwidth = 0;
        window = 0f;
    }
}
