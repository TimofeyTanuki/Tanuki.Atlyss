using Steamworks;
using System.Collections.Generic;
using Tanuki.Atlyss.Network.Types;
using Tanuki.Atlyss.Network.Types.Packets;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Services;

public sealed class WindowRateLimiter(float window, uint bandwidth) : IRateLimiter
{
    private readonly Dictionary<ulong, RateLimitEntry> entries = [];

    private float nextRefresh = 0;

    public void Refresh()
    {
        float time = Time.unscaledTime;

        if (nextRefresh > time)
            return;

        nextRefresh = time + window;
    }

    public bool CheckBandwidthOverflow(CSteamID sender, uint usage)
    {
        if (entries.TryGetValue(sender.m_SteamID, out RateLimitEntry entry))
        {
            if (entry.NextRefresh < nextRefresh)
            {
                entry.Usage = 0;
                entry.NextRefresh = nextRefresh;
            }
        }
        else
        {
            entry = new() { NextRefresh = 0 };
            entries.Add(sender.m_SteamID, entry);
        }

        entry.Usage += usage;

        return entry.Usage > bandwidth;
    }

    public void Reset(ulong sender) => entries.Remove(sender);

    public void Reset() => entries.Clear();
}
