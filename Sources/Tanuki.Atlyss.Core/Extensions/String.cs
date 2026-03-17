using System;
using System.Collections.Generic;
using System.Text;

namespace Tanuki.Atlyss.Core.Extensions;

public static class String
{
    extension(string instance)
    {
        public static string Join(string separator, IReadOnlyList<string> value, int startIndex, int count)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (startIndex < 0 || startIndex > value.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || startIndex + count > value.Count)
                throw new ArgumentOutOfRangeException(nameof(count));

            count = Math.Min(count, value.Count - startIndex);

            StringBuilder stringBuilder = new();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    stringBuilder.Append(separator);

                stringBuilder.Append(value[i]);
            }

            return stringBuilder.ToString();
        }

        public static string Join(string separator, IReadOnlyList<string> value, int startIndex) =>
            Join(separator, value, startIndex, value.Count - startIndex);
    }
}
