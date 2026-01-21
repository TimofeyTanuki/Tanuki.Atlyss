using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using Tanuki.Atlyss.API.Network.Compression;

namespace Tanuki.Atlyss.Network.Providers.Compression;

/// <summary>
/// Provides data compression using the Brotli algorithm.
/// </summary>
/// <param name="compressionLevel">
/// Specifies the compression level used by the encoder.
/// </param>
/// <param name="maxCompressedDataSize">
/// Defines the maximum expected size of compressed data.
/// This value shouldn't be set too high, as reserving a large memory buffer can degrade performance and may be abused in malicious scenarios.
/// </param>
public sealed class Brotli(CompressionLevel compressionLevel = CompressionLevel.Fastest, int maxCompressedDataSize = 16384) : ICompressionProvider
{
    private const int HEADER_SIZE = sizeof(int);
    private readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;

    private readonly CompressionLevel compressionLevel = compressionLevel;
    private readonly int maxCompressedDataSize = maxCompressedDataSize;

    public int GetCompressedSize(int inputSize) => inputSize + (inputSize / 16) + 64;

    public int GetDecompressedSize(ReadOnlySpan<byte> compressedData)
    {
        if (compressedData.Length < HEADER_SIZE)
            throw new InvalidOperationException("Compressed data is too short to contain a valid header.");

        return (int)BinaryPrimitives.ReadUInt32LittleEndian(compressedData);
    }

    public int Compress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(output[..HEADER_SIZE], (uint)input.Length);

        using MemoryStream memoryStream = new(output[HEADER_SIZE..].ToArray());
        using (BrotliStream brotliStream = new(memoryStream, compressionLevel, true))
            brotliStream.Write(input);

        int compressedLength = (int)memoryStream.Position;

        memoryStream.Position = 0;
        memoryStream.Read(output.Slice(HEADER_SIZE, compressedLength));

        int length = HEADER_SIZE + compressedLength;

        if (length > maxCompressedDataSize)
            throw new InvalidOperationException("Compressed data exceeds the compressor's maximum allowed size.");

        return length;
    }

    public int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        if (input.Length < HEADER_SIZE)
            throw new InvalidOperationException("Input data is too short to contain a valid header.");

        int originalSize = (int)BinaryPrimitives.ReadUInt32LittleEndian(input);

        if (originalSize > maxCompressedDataSize)
            throw new InvalidOperationException("Original size from header exceeds the maximum allowed compressed size.");

        if (output.Length < originalSize)
            throw new InvalidOperationException("Output buffer is too small to hold the decompressed data.");

        ReadOnlySpan<byte> compressedData = input[HEADER_SIZE..];

        byte[] buffer = arrayPool.Rent(compressedData.Length);
        int totalRead = 0;
        try
        {
            compressedData.CopyTo(buffer);
            using MemoryStream memoryStream = new(buffer, 0, compressedData.Length, writable: false);
            using BrotliStream brotliStream = new(memoryStream, CompressionMode.Decompress);

            while (totalRead < originalSize)
            {
                int read = brotliStream.Read(output[totalRead..]);
                if (read == 0) break;
                totalRead += read;
            }

            if (totalRead != originalSize)
                throw new InvalidOperationException("Decompressed data size doesn't match the expected original size.");

            return totalRead;
        }
        finally
        {
            arrayPool.Return(buffer);
        }
    }
}
