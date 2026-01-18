using BepInEx.Logging;
using System;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Network.Data.Packets;

namespace Tanuki.Atlyss.Network.Managers;

public sealed class Packets
{
    private readonly ManualLogSource manualLogSource;
    private readonly Registers.Packets packetRegistry;

    internal Packets(ManualLogSource manualLogSource, Registers.Packets packetRegistry)
    {
        this.manualLogSource = manualLogSource;
        this.packetRegistry = packetRegistry;
    }

    public void AddHandler<T>(API.Network.Packets.Handler handler)
        where T : Packet
    {
        Type type = typeof(T);

        if (!packetRegistry.PacketEntries.TryGetValue(type, out RegistryEntry registryEntry))
        {
            manualLogSource.LogWarning($"Cannot add handler for packet {type.FullName} because it isn't registered.");
            return;
        }

        registryEntry.PacketHandlers.Add(handler);
    }

    public void RemoveHandler<T>(API.Network.Packets.Handler handler)
        where T : Packet
    {
        Type type = typeof(T);

        if (!packetRegistry.PacketEntries.TryGetValue(type, out RegistryEntry registryEntry))
            return;

        registryEntry.PacketHandlers.Remove(handler);
    }
}
