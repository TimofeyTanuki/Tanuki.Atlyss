using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Utilities.Commands;

public static class Name
{
    public static string Normalize(string? raw, IReadOnlyList<string> prefixes)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        ReadOnlySpan<char> span = raw.AsSpan();

        bool prefixFound;
        do
        {
            prefixFound = false;

            foreach (string prefix in prefixes)
            {
                if (span.StartsWith(prefix.AsSpan(), StringComparison.Ordinal))
                {
                    span = span[prefix.Length..];

                    prefixFound = true;
                    break;
                }
            }
        } while (prefixFound);

        int spanLength = span.Length;
        char[] buffer = new char[spanLength];
        int charactersCount = 0;

        for (int index = 0; index < spanLength; index++)
        {
            if (char.IsWhiteSpace(span[index]))
                continue;

            buffer[charactersCount++] = span[index];
        }

        return new string(buffer, 0, charactersCount);
    }
}