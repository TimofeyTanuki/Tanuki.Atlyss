using BepInEx.Logging;
using Steamworks;
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

    public void AddHandler<TPacket>(Action<CSteamID, TPacket> handler)
        where TPacket : Packet, new()
    {
        Type type = typeof(TPacket);

        if (!packetRegistry.PacketDescriptors.TryGetValue(type, out Descriptor descriptor))
        {
            manualLogSource.LogError($"Failed to add handler for packet {type.FullName} because it isn't registered.");
            return;
        }

        if (descriptor is not Descriptor<TPacket> typedDescriptor)
        {
            manualLogSource.LogError($"Failed to add handler for packet {type.FullName} because packet descriptor type doesn't match the expected handler type.");
            return;
        }

        typedDescriptor.PacketHandlers.Add(handler);
    }

    public void RemoveHandler<TPacket>(Action<CSteamID, TPacket> handler)
        where TPacket : Packet, new()
    {
        Type type = typeof(TPacket);

        if (!packetRegistry.PacketDescriptors.TryGetValue(type, out Descriptor descriptor))
        {
            manualLogSource.LogError($"Failed to add handler for packet {type.FullName} because it isn't registered.");
            return;
        }

        if (descriptor is not Descriptor<TPacket> typedDescriptor)
        {
            manualLogSource.LogError($"Failed to add handler for packet {type.FullName} because packet descriptor type doesn't match the expected handler type.");
            return;
        }

        typedDescriptor.PacketHandlers.Remove(handler);
    }

    public void ChangeMuteState<TPacket>(bool isMuted)
        where TPacket : Packet, new()
    {
        Type type = typeof(TPacket);

        if (!packetRegistry.PacketDescriptors.TryGetValue(type, out Descriptor descriptor))
        {
            manualLogSource.LogError($"Failed to change mute status for packet {type.FullName} because it isn't registered.");
            return;
        }

        descriptor.isMuted = isMuted;
    }
}
