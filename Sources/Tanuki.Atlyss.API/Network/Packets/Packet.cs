using System;

namespace Tanuki.Atlyss.API.Network.Packets;

public abstract class Packet
{
    /// <returns>
    /// The maximum number of bytes required for serialization.
    /// </returns>
    public abstract int GetMaxSize();

    /// <param name="buffer">
    /// Buffer with a size no less than <see cref="GetMaxSize"/>.
    /// </param>
    /// <exception cref="Exception">
    /// Exception related to serialization.
    /// </exception>
    public abstract int Serialize(Span<byte> buffer);

    /// <param name="buffer">
    /// Buffer with serialized data.
    /// </param>
    /// <exception cref="Exception">
    /// Exception related to deserialization.
    /// </exception>
    public abstract void Deserialize(ReadOnlySpan<byte> buffer);
}
