using System;

namespace Tanuki.Atlyss.Core.Utilities.Commands;

public static class Hash
{
    public static ulong Generate(Type type) => Shared.Cryptography.FNV64.Compute(type.FullName);

    public static ulong Generate<T>() => Generate(typeof(T));
}
