using System;
using System.Buffers;
using System.Buffers.Binary;
using Tanuki.Atlyss.API.Network.Compression;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Network.Data.Packets;

namespace Tanuki.Atlyss.Network.Services;

public sealed class PacketProcessor
{
    private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;
    private readonly Providers.Packets packetProvider;

    internal PacketProcessor(Providers.Packets packetProvider) => this.packetProvider = packetProvider;

    /// <remarks>
    /// <see cref="PackedPacket"/> must be disposed after use.
    /// </remarks>
    public PackedPacket Pack<TPacket>(Descriptor descriptor, TPacket packet)
        where TPacket : Packet, new()
    {
        int maxDataSize = packet.GetMaxSize();
        byte[] dataBuffer = arrayPool.Rent(maxDataSize);
        byte[]? compressedBuffer = null;
        ICompressionProvider? compressionProvider = descriptor.CompressionProvider;

        try
        {
            int dataLength = packet.Serialize(dataBuffer.AsSpan(0, maxDataSize));

            ReadOnlySpan<byte> data = dataBuffer.AsSpan(0, dataLength);

            if (compressionProvider is not null)
            {
                int maxCompressedSize = compressionProvider.GetCompressedSize(dataLength);
                compressedBuffer = arrayPool.Rent(maxCompressedSize);

                int compressedLength = compressionProvider.Compress(
                    data,
                    compressedBuffer.AsSpan(0, maxCompressedSize)
                );

                data = compressedBuffer.AsSpan(0, compressedLength);
            }

            int totalLength = Tanuki.PACKET_SIGNATURE_SIZE + data.Length;
            byte[] packetBuffer = arrayPool.Rent(totalLength);

            BinaryPrimitives.WriteUInt64LittleEndian(
                packetBuffer.AsSpan(0, Tanuki.PACKET_SIGNATURE_SIZE),
                descriptor.Signature
            );

            data.CopyTo(packetBuffer.AsSpan(Tanuki.PACKET_SIGNATURE_SIZE));

            return new PackedPacket(packetBuffer, totalLength);
        }
        catch
        {
            throw;
        }
        finally
        {
            arrayPool.Return(dataBuffer);

            if (compressedBuffer is not null)
                arrayPool.Return(compressedBuffer);
        }
    }

    /// <remarks>
    /// Data size and signature aren't checked during operation.
    /// </remarks>
    public TPacket Unpack<TPacket>(Descriptor descriptor, ReadOnlySpan<byte> packetData)
        where TPacket : Packet, new()
    {
        ReadOnlySpan<byte> payload = packetData[Tanuki.PACKET_SIGNATURE_SIZE..];
        byte[]? decompressedBuffer = null;
        ICompressionProvider? compressionProvider = descriptor.CompressionProvider;

        try
        {
            if (compressionProvider is not null)
            {
                int maxDecompressedSize =
                    compressionProvider.GetDecompressedSize(payload);

                decompressedBuffer = arrayPool.Rent(maxDecompressedSize);

                int decompressedLength = compressionProvider.Decompress(
                    payload,
                    decompressedBuffer.AsSpan(0, maxDecompressedSize)
                );

                payload = decompressedBuffer.AsSpan(0, decompressedLength);
            }

            TPacket packet = packetProvider.Create<TPacket>();
            packet.Deserialize(payload);
            return packet;
        }
        catch
        {
            throw;
        }
        finally
        {
            if (decompressedBuffer is not null)
                arrayPool.Return(decompressedBuffer);
        }
    }
}
