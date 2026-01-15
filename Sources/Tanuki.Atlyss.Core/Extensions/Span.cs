using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Extensions;

public static class Span
{
    extension(Span<char> Span)
    {
        public Span<char> RemoveCharacters(ISet<char> Set)
        {
            int Length = 0;

            for (int Index = 0; Index < Span.Length; Index++)
            {
                char Character = Span[Index];

                if (!Set.Contains(Character))
                    Span[Length++] = Character;
            }

            return Span[..Length];
        }
    }
}