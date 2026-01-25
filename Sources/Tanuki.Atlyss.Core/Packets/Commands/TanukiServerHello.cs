using System;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Shared.Serialization;

namespace Tanuki.Atlyss.Core.Packets.Commands;

public sealed class TanukiServerHello : Packet
{
    private readonly int MAX_SIZE =
        Encoding.UTF8.GetByteCount(PluginInfo.VERSION) + Data.Settings.Commands.SERVER_PREFIX_MAX_LENGTH + 64;

    public string? Version = null;
    public string? ServerCommandPrefix = null;

    public override int GetMaxSize() => MAX_SIZE;

    public override int Serialize(Span<byte> buffer)
    {
        Encoding encoding = Encoding.UTF8;
        buffer.Clear();

        int offset = 0;
        offset += BinaryString.WriteNullTerminated(buffer[offset..], Version.AsSpan(), encoding);
        offset += BinaryString.WriteNullTerminated(buffer[offset..], Tanuki.instance.providers.settings.CommandSection.serverPrefix, encoding);

        return offset;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Encoding encoding = Encoding.UTF8;

        Version = BinaryString.ReadNullTerminated(buffer, encoding);

        if (Version is null)
            return;

        if (Version != PluginInfo.VERSION)
            return;
    }
}
