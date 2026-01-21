using System;

namespace Tanuki.Atlyss.API.Network.Compression;

public interface ICompressionProvider
{
    /// <returns>
    /// The maximum number of bytes required for compressed data.
    /// </returns>
    public int GetCompressedSize(int inputSize);

    /// <returns>
    /// The maximum number of bytes required for decompression.
    /// </returns>
    public int GetDecompressedSize(ReadOnlySpan<byte> compressedData);

    public int Compress(ReadOnlySpan<byte> input, Span<byte> output);

    public int Decompress(ReadOnlySpan<byte> input, Span<byte> output);
}
