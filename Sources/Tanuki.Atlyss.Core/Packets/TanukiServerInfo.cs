using System;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Shared.Serialization;

namespace Tanuki.Atlyss.Core.Packets;

public sealed class TanukiServerInfo : Packet
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    public string? Version = null;
    public string? ServerCommandPrefix = null;

    public override int GetMaxSize()
    {
        int size = 0;

        size += Encoding.GetByteCount(Version) + 1;
        size += Encoding.GetByteCount(ServerCommandPrefix) + 1;

        return size;
    }

    public override int Serialize(Span<byte> buffer)
    {
        int offset = 0;

        offset += BinaryString.WriteNullTerminated(buffer[offset..], Version.AsSpan(), Encoding);
        offset += BinaryString.WriteNullTerminated(buffer[offset..], ServerCommandPrefix.AsSpan(), Encoding);

        return offset;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        int offset = 0;

        Version = BinaryString.ReadNullTerminated(buffer, Encoding);

        if (string.IsNullOrEmpty(Version))
            return;

        offset += Encoding.GetByteCount(Version) + 1;

        if (Version != PluginInfo.VERSION)
            return;

        ServerCommandPrefix = BinaryString.ReadNullTerminated(buffer[offset..], Encoding);
    }
}
