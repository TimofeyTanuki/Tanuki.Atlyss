using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Tanuki.Atlyss.API.Network.Packets;
using Tanuki.Atlyss.Shared.Serialization;

namespace Tanuki.Atlyss.Core.Packets.Commands;

public sealed class Request : Packet
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    public string? Name;
    public ulong? Hash;
    public IReadOnlyList<string> Arguments = null!;

    public override int GetMaxSize()
    {
        int size = 1;

        if (Hash.HasValue)
            size += Network.Tanuki.PACKET_SIGNATURE_SIZE;
        else
            size += Encoding.GetByteCount(Name) + 1;

        if (Arguments is not null)
        {
            foreach (string argument in Arguments)
            {
                if (string.IsNullOrEmpty(argument))
                    continue;

                size += Encoding.GetByteCount(argument) + 1;
            }
        }

        return size;
    }

    public override int Serialize(Span<byte> buffer)
    {
        int offset = 0;

        if (Hash.HasValue)
        {
            buffer[offset++] = 1;
            BitConverter.TryWriteBytes(buffer.Slice(offset, Network.Tanuki.PACKET_SIGNATURE_SIZE), Hash.Value);
            offset += Network.Tanuki.PACKET_SIGNATURE_SIZE;
        }
        else
        {
            buffer[offset++] = 0;
            offset += BinaryString.WriteNullTerminated(buffer[offset..], Name, Encoding);
        }

        if (Arguments is not null)
        {
            foreach (string argument in Arguments)
            {
                if (string.IsNullOrEmpty(argument))
                    continue;

                offset += BinaryString.WriteNullTerminated(buffer[offset..], argument, Encoding);
            }
        }

        return offset;
    }

    public override void Deserialize(ReadOnlySpan<byte> data)
    {
        int offset = 0;

        byte flag = data[offset++];

        if (flag == 1)
        {
            Hash = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, Network.Tanuki.PACKET_SIGNATURE_SIZE));
            offset += Network.Tanuki.PACKET_SIGNATURE_SIZE;
            Name = null;
        }
        else
        {
            Hash = null;
            Name = BinaryString.ReadNullTerminated(data[offset..], Encoding);
            offset += Name != null ? Encoding.GetByteCount(Name) + 1 : 0;
        }

        List<string> arguments = [];

        while (offset < data.Length)
        {
            string? argument = BinaryString.ReadNullTerminated(data[offset..], Encoding);

            if (string.IsNullOrEmpty(argument))
                continue;

            arguments.Add(argument);
            offset += Encoding.GetByteCount(argument) + 1;
        }

        Arguments = arguments;
    }
}
