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
    private readonly Dictionary<Type, RegistryEntry> packetEntries = [];

    public IReadOnlyDictionary<ulong, Type> HashMap => hashMap;
    public IReadOnlyDictionary<Type, RegistryEntry> PacketEntries => packetEntries;

    internal Packets(ManualLogSource manualLogSource) => this.manualLogSource = manualLogSource;

    public void Register<T>(ICompressionProvider? compressionProvider)
        where T : Packet
    {
        Type type = typeof(T);

        if (packetEntries.ContainsKey(type))
        {
            manualLogSource.LogWarning($"Packet {type.FullName} is already registered.");
            return;
        }

        ulong signature = Shared.Cryptography.FNV64.Compute(type.FullName);

        if (hashMap.ContainsKey(signature))
        {
            manualLogSource.LogWarning($"Packet with Signature {signature} is alredy registered.");
            return;
        }

        hashMap.Add(signature, type);
        packetEntries.Add(
            type,
            new(signature)
            {
                CompressionProvider = compressionProvider
            }
        );
    }

    public void Deregister<T>()
        where T : Packet
    {
        Type type = typeof(T);

        if (!packetEntries.TryGetValue(type, out RegistryEntry registryEntry))
            return;

        hashMap.Remove(registryEntry.Signature);
        packetEntries.Remove(type);
    }
}
