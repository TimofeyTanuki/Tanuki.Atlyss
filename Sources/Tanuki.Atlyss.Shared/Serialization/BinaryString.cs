using System;
using System.Text;

namespace Tanuki.Atlyss.Shared.Serialization;

public sealed class BinaryString
{
    public static string? ReadNullTerminated(ReadOnlySpan<byte> buffer, Encoding encoding)
    {
        int length = buffer.IndexOf((byte)0);

        if (length < 0)
            return null;

        return encoding.GetString(buffer[..length]);
    }

    public static int WriteNullTerminated(Span<byte> buffer, ReadOnlySpan<char> value, Encoding encoding)
    {
        int offset = encoding.GetBytes(value, buffer);
        buffer[offset] = 0;
        return offset + 1;
    }
}
