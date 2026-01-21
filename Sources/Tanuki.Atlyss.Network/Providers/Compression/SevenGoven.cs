using System;
using Tanuki.Atlyss.API.Network.Compression;

namespace Tanuki.Atlyss.Network.Providers.Compression;

/// <summary>
/// SevenGoven is a boldly inefficient compressor that increases data size exactly eightfold by carefully adding serven bytes of high-quality random before every meaningful byte.
/// Designed to challenge conventional ideas of compression, performance, and sanity.
/// Original idea by <see href="https://github.com/iBowie">BowieD</see>.
/// </summary>
public sealed class SevenGoven() : ICompressionProvider
{
    public int GetCompressedSize(int inputSize) => inputSize * 8;

    public int GetDecompressedSize(ReadOnlySpan<byte> compressedData) => compressedData.Length / 8;

    public int Compress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int offset = 0;
        foreach (byte original in input)
        {
            for (int index = 0; index < 7; index++)
                output[offset++] = (byte)UnityEngine.Random.Range(byte.MinValue, byte.MaxValue);

            output[offset++] = original;
        }

        return offset;
    }

    public int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int offset = 0;
        for (int index = 7; index < input.Length; index += 8)
            output[offset++] = input[index];

        return offset;
    }
}
