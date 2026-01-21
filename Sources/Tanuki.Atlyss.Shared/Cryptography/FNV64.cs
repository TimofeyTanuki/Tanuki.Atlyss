using System;
using System.Runtime.CompilerServices;

namespace Tanuki.Atlyss.Shared.Cryptography;

public static class FNV64
{
    private const ulong
        offset = 14695981039346656037UL,
        prime = 1099511628211UL;

    public static ulong Compute(ReadOnlySpan<char> input)
    {
        ulong hash = offset;

        for (int index = 0; index < input.Length; index++)
        {
            hash ^= input[index];
            hash *= prime;
        }

        return hash;
    }
}
