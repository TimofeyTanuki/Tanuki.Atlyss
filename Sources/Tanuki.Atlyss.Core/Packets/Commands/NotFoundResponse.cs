using System;
using Tanuki.Atlyss.API.Network.Packets;

namespace Tanuki.Atlyss.Core.Packets.Commands;

public sealed class NotFoundResponse : Packet
{
    public override int GetMaxSize() => 0;

    public override void Deserialize(ReadOnlySpan<byte> buffer) { }

    public override int Serialize(Span<byte> buffer) => 0;
}