using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Shared.Extensions;

public static class Span
{
    extension(Span<char> span)
    {
        public Span<char> RemoveCharacters(ISet<char> set)
        {
            int length = 0;

            for (int index = 0; index < span.Length; index++)
            {
                char character = span[index];

                if (!set.Contains(character))
                    span[length++] = character;
            }

            return span[..length];
        }
    }
}