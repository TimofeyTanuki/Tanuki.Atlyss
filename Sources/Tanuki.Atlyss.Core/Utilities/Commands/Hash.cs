using System;

namespace Tanuki.Atlyss.Core.Utilities.Commands;

public static class Hash
{
    private const ulong
        offset = 14695981039346656037UL,
        prime = 1099511628211UL;

    public static ulong Generate(Type type)
    {
        ulong hash = offset;

        foreach (char character in type.FullName)
        {
            hash ^= character;
            hash *= prime;
        }

        return hash;
    }

    public static ulong Generate<T>() => Generate(typeof(T));
}
