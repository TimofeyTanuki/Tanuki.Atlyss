namespace Tanuki.Atlyss.Core.Packets.Commands;

/*
internal class CommandRequest : IPacket
{
    public string? Name;
    public ulong? Hash;
    public IReadOnlyList<string>? Arguments;

    public override int GetSize()
    {
        int size = 1;

        if (Name is not null)
            size += 2 + Encoding.UTF8.GetByteCount(Name);
        else if (Hash is not null)
            size += sizeof(ulong);
        else
            throw new InvalidOperationException("Name or Hash must be set.");

        size += 2;

        if (Arguments is not null)
            foreach (string argument in Arguments)
                size += 2 + Encoding.UTF8.GetByteCount(argument);

        return size;
    }

    public override void Serialize(Span<byte> buffer)
    {
        int offset = 0;

        byte requrestType = Name is null ? (byte)1 : (byte)2;
        buffer[offset++] = requrestType;

        if (Name is not null)
        {
            ushort length = (ushort)Encoding.UTF8.GetByteCount(Name);
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, length);
            offset += 2;

            Encoding.UTF8.GetBytes(Name, buffer.Slice(offset, length));
            offset += length;
        }
        else
            BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(offset, 8), Hash!.Value);

        ushort count = (ushort)(Arguments?.Count ?? 0);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset, 2), count);

        offset += 2;
        
        if (Arguments is not null)
        {
            foreach (string argument in Arguments)
            {
                ushort length = (ushort)Encoding.UTF8.GetByteCount(argument);
                BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset, 2), length);
                offset += 2;

                Encoding.UTF8.GetBytes(argument, buffer.Slice(offset, length));
                offset += length;
            }
        }
    }

    public override void Deserialize(ReadOnlySpan<byte> data)
    {
        int offset = 0;

        byte requestType = data[offset++];

        if ((requestType & 1) != 0)
        {
            ushort len = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;

            Name = Encoding.UTF8.GetString(data.Slice(offset, len));
            offset += len;

            Hash = null;
        }
        else if ((requestType & 2) != 0)
        {
            Hash = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8));
            offset += 8;

            Name = null;
        }
        else
            throw new InvalidDataException("Invalid requrest type.");

        ushort count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (count == 0)
        {
            Arguments = [];
            return;
        }

        string[] arguments = new string[count];
        for (int i = 0; i < count; i++)
        {
            ushort len = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;

            arguments[i] = Encoding.UTF8.GetString(data.Slice(offset, len));
            offset += len;
        }

        Arguments = arguments;
    }
}
*/