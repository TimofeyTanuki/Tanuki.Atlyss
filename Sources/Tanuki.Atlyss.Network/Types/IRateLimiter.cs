using Steamworks;

namespace Tanuki.Atlyss.Network.Types;

public interface IRateLimiter
{
    public void Refresh();
    public bool CheckBandwidthOverflow(CSteamID sender, uint usage);
    public void Reset(ulong sender);
    public void Reset();
}
