using Steamworks;
using System.Collections.Generic;
using Tanuki.Atlyss.Network.Data.Packets;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Services;

public sealed class RateLimiter
{
    private readonly Dictionary<ulong, RateLimitEntry> entries = [];
    public float Window;
    public uint Bandwidth;

    private float nextRefresh = 0;

    public void Tick()
    {
        float time = Time.unscaledTime;

        if (nextRefresh > time)
            return;

        nextRefresh = time + Window;
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

        return entry.Usage > Bandwidth;
    }

    public void Reset(ulong sender) => entries.Remove(sender);

    public void Reset() => entries.Clear();
}
