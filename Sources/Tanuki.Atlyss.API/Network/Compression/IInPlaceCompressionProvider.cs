using System;

namespace Tanuki.Atlyss.API.Network.Compression;

public interface IInPlaceCompressionProvider : ICompressionProvider
{
    int Compress(Span<byte> data, int length);

    int Decompress(Span<byte> data, int length);
}
