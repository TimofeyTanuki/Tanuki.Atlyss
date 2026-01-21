using System;
using System.IO;
using System.Text;
using Tanuki.Atlyss.API.Network.Compression;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Providers.Compression;

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
