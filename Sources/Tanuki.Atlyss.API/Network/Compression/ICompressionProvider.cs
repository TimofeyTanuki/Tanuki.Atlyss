using System;

namespace Tanuki.Atlyss.API.Network.Compression;

public interface ICompressionProvider
{
    public int Compress(ReadOnlySpan<byte> input, Span<byte> output);

    public int Decompress(ReadOnlySpan<byte> input, Span<byte> output);
}
