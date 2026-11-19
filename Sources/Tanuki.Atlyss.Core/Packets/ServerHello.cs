using System;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Shared.Serialization;

namespace Tanuki.Atlyss.Core.Packets;

public sealed class ServerHello : Packet
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    public string? Version = null;

    public override int GetMaxSize() =>
        Encoding.GetByteCount(Version) + 1;

    public override int Serialize(Span<byte> buffer) =>
        BinaryString.WriteNullTerminated(buffer, Version.AsSpan(), Encoding);

    public override void Deserialize(ReadOnlySpan<byte> buffer) =>
        Version = BinaryString.ReadNullTerminated(buffer, Encoding);
}
