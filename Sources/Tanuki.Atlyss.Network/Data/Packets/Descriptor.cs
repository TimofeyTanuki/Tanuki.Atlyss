using Steamworks;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.API.Network.Compression;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Network.Services;

namespace Tanuki.Atlyss.Network.Data.Packets;

public abstract class Descriptor(ulong signature, ICompressionProvider? compressionProvider)
{
    public readonly ulong Signature = signature;
    public readonly ICompressionProvider? CompressionProvider = compressionProvider;

    public abstract void ProcessPacket(PacketProcessor packetProcessor, CSteamID sender, ReadOnlySpan<byte> packetData);
    //public abstract void PackPacket()
}

public sealed class Descriptor<TPacket>(ulong signature, ICompressionProvider? compressionProvider) : Descriptor(signature, compressionProvider)
    where TPacket : Packet, new()
{
    internal List<Action<CSteamID, TPacket>> PacketHandlers = [];

    public void Dispatch(CSteamID sender, TPacket packet)
    {
        foreach (var handler in PacketHandlers)
            handler(sender, packet);
    }

    public override void ProcessPacket(PacketProcessor packetProcessor, CSteamID sender, ReadOnlySpan<byte> packetData)
    {
        TPacket packet;

        try
        {
            packet = packetProcessor.Unpack<TPacket>(this, packetData);
        }
        catch
        {
            throw;
        }

        Dispatch(sender, packet);
    }
}
