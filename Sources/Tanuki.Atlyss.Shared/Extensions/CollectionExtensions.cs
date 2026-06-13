using System;

namespace Tanuki.Atlyss.Shared.Extensions;

public static class CollectionExtensions
{
    extension<T>(T[] collection)
        where T : IComparable<T>
    {
        public bool ContainsAll(T[] items)
        {
            int
                index = 0,
                itemIndex = 0;

            while (index < collection.Length && itemIndex < items.Length)
            {
                int comparisonResult = collection[index].CompareTo(items[itemIndex]);

                if (comparisonResult < 0)
                    index++;
                else if (comparisonResult > 0)
                    return false;
                else
                {
                    index++;
                    itemIndex++;
                }
            }

            return itemIndex == items.Length;
        }

        public bool SequenceEquals(T[] other)
        {
            if (collection.Length != other.Length)
                return false;

            for (int index = 0; index < collection.Length; index++)
                if (!collection[index].Equals(other[index]))
                    return false;

            return true;
        }
    }
}
