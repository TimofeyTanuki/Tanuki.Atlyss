using System;

namespace Tanuki.Atlyss.API.Commands;

public readonly struct Hash : IEquatable<Hash>
{
    public readonly ulong A, B, C, D;

    public Hash(byte[] Hash)
    {
        if (Hash.Length != 32)
            throw new ArgumentException("The hash must consist of 32 bytes.");

        A = BitConverter.ToUInt64(Hash, 0);
        B = BitConverter.ToUInt64(Hash, 8);
        C = BitConverter.ToUInt64(Hash, 16);
        D = BitConverter.ToUInt64(Hash, 24);
    }

    public readonly bool Equals(Hash Hash) =>
        A == Hash.A && B == Hash.B && C == Hash.C && D == Hash.D;

    public override readonly int GetHashCode() => HashCode.Combine(A, B, C, D);
}