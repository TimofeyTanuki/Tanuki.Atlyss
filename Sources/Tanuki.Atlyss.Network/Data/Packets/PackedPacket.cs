using System;
using System.Buffers;

namespace Tanuki.Atlyss.Network.Data.Packets;

public readonly struct PackedPacket : IDisposable
{
    private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;

    private readonly byte[] buffer;
    private readonly int length;

    public ArraySegment<byte> ArraySegment => new(buffer, 0, length);
    public ReadOnlySpan<byte> Array => buffer.AsSpan(0, length);
    public int Length => length;

    internal PackedPacket(byte[] buffer, int length)
    {
        this.buffer = buffer;
        this.length = length;
    }

    public readonly void Dispose()
    {
        if (buffer is not null)
            arrayPool.Return(buffer);
    }
}
