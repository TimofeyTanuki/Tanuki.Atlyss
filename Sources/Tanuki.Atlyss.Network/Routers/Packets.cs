using BepInEx.Logging;
using Steamworks;
using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Network.Data.Packets;
using Tanuki.Atlyss.Network.Services;

namespace Tanuki.Atlyss.Network.Routers;

public sealed class Packets
{
    private readonly ManualLogSource manualLogSource;
    private readonly Registers.Packets packetRegistry;
    private readonly PacketProcessor packetProcessor;
    private readonly Providers.SteamLobby steamLobbyProvider;

    internal int steamLocalChannel;

    internal Packets(
        ManualLogSource manualLogSource,
        Registers.Packets packetRegistry,
        PacketProcessor packetProcessor,
        Providers.SteamLobby steamLobbyProvider)
    {
        this.manualLogSource = manualLogSource;
        this.packetRegistry = packetRegistry;
        this.packetProcessor = packetProcessor;
        this.steamLobbyProvider = steamLobbyProvider;
    }

    private bool CreatePackedPacket<TPacket>(TPacket packet, out PackedPacket packedPacket)
        where TPacket : Packet, new()
    {
        packedPacket = default;
        Type type = typeof(TPacket);

        if (!packetRegistry.PacketDescriptors.TryGetValue(type, out Descriptor descriptor))
        {
            manualLogSource.LogError($"Packet {type.FullName} isn't registered and can't be sent.");
            return false;
        }

        if (descriptor is not Descriptor<TPacket>)
        {
            manualLogSource.LogError($"Packet {type.FullName} doesn't match the expected registered type.");
            return false;
        }

        try
        {
            packedPacket = packetProcessor.Pack(descriptor, packet);
        }
        catch (Exception exception)
        {
            manualLogSource.LogError($"Failed to pack packet {type.FullName}.\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            return false;
        }

        return true;
    }

    public void SendPacketToLobbyChat<TPacket>(TPacket packet)
        where TPacket : Packet, new()
    {
        if (!CreatePackedPacket(packet, out PackedPacket packedPacket))
            return;

        SteamMatchmaking.SendLobbyChatMsg(steamLobbyProvider.SteamId, packedPacket.ArraySegment.Array, packedPacket.Length);
        packedPacket.Dispose();
    }

    public bool SendPacketToUser<TPacket>(CSteamID target, TPacket packet, int remoteChannel, out EResult steamResult)
        where TPacket : Packet, new()
    {
        steamResult = default;

        if (!CreatePackedPacket(packet, out PackedPacket packedPacket))
            return false;

        if (packedPacket.Length <= Tanuki.PACKET_MAX_SIZE)
        {
            SteamNetworkingIdentity steamNetworkingIdentity = new();
            steamNetworkingIdentity.SetSteamID(target);

            GCHandle packetBytesHandle = GCHandle.Alloc(packedPacket.ArraySegment.Array, GCHandleType.Pinned);

            try
            {
                IntPtr dataPointer = packetBytesHandle.AddrOfPinnedObject();
                steamResult = SteamNetworkingMessages.SendMessageToUser(ref steamNetworkingIdentity, dataPointer, (uint)packedPacket.Length, 0, remoteChannel);
            }
            catch (Exception exception)
            {
                manualLogSource.LogError($"An exception occurred while sending packet {packet.GetType().FullName}.\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
            }
            finally
            {
                packetBytesHandle.Free();
            }
        }
        else
            manualLogSource.LogError($"Packet {packet.GetType().FullName} exceeds the maximum allowed size of {Tanuki.PACKET_MAX_SIZE} bytes and can't be sent.");

        packedPacket.Dispose();

        return true;
    }

    public bool SendPacketToUser<TPacket>(CSteamID target, TPacket packet, out EResult steamResult)
        where TPacket : Packet, new() => SendPacketToUser(target, packet, steamLocalChannel, out steamResult);

    public void ReceivePacket(CSteamID sender, ReadOnlySpan<byte> data)
    {
        if (data.Length < Tanuki.PACKET_SIGNATURE_SIZE)
            return;

        ulong signature = BinaryPrimitives.ReadUInt64LittleEndian(data[..Tanuki.PACKET_SIGNATURE_SIZE]);

        if (!packetRegistry.HashMap.TryGetValue(signature, out Type packetType))
            return;

        Descriptor descriptor = packetRegistry.PacketDescriptors[packetType];

        if (descriptor.isMuted)
            return;

        try
        {
            descriptor.ProcessPacket(packetProcessor, sender, data);
        }
        catch (Exception exception)
        {
            manualLogSource.LogError($"Failed to process packet {packetType.FullName}.\nException:\n{exception.Message}\nStack trace:\n{exception.StackTrace}");
        }
    }
}
