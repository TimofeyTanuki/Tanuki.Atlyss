using System;

namespace Tanuki.Atlyss.API.Network.Packets;

public abstract class Packet
{
    /// <returns>
    /// Size of the buffer required for serializing the packet.
    /// </returns>
    public abstract int GetSize();

    public abstract void Serialize(Span<byte> buffer);

    public abstract void Deserialize(ReadOnlySpan<byte> data);
}
