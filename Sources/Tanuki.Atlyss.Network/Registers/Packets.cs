using BepInEx.Logging;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Network.Compression;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Network.Data.Packets;

namespace Tanuki.Atlyss.Network.Registers;

public sealed class Packets
{
    private readonly ManualLogSource manualLogSource;
    private readonly Dictionary<ulong, Type> hashMap = [];
    private readonly Dictionary<Type, Descriptor> packetDescriptors = [];

    public IReadOnlyDictionary<ulong, Type> HashMap => hashMap;
    public IReadOnlyDictionary<Type, Descriptor> PacketDescriptors => packetDescriptors;

    internal Packets(ManualLogSource manualLogSource) => this.manualLogSource = manualLogSource;

    public void Register<TPacket>(ICompressionProvider? compressionProvider = null)
        where TPacket : Packet, new()
    {
        Type type = typeof(TPacket);

        if (packetDescriptors.TryGetValue(type, out Descriptor descriptor))
        {
            if (compressionProvider != descriptor.CompressionProvider)
                manualLogSource.LogWarning($"Packet {type.FullName} is already registered with another compression provider.");

            return;
        }

        ulong signature = Shared.Cryptography.FNV64.Compute(type.FullName);

        if (hashMap.ContainsKey(signature))
        {
            manualLogSource.LogWarning($"Packet with signature {signature} is alredy registered.");
            return;
        }

        hashMap.Add(signature, type);
        packetDescriptors.Add(
            type,
            new Descriptor<TPacket>(signature, compressionProvider)
        );
    }

    public void Deregister<T>()
        where T : Packet
    {
        Type type = typeof(T);

        if (!packetDescriptors.TryGetValue(type, out Descriptor descriptor))
            return;

        hashMap.Remove(descriptor.Signature);
        packetDescriptors.Remove(type);
    }
}
