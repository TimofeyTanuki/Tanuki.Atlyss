using System;

namespace Tanuki.Atlyss.Core.Types.Managers.Hotkey;

public sealed class KeyCombination
{
    public readonly KeyCondition[] KeyTriggers;

    public KeyCombination(params KeyCondition[] keyConditions)
    {
        if (keyConditions is null || keyConditions.Length == 0)
            throw new ArgumentException("At least one key trigger must be provided.", nameof(keyConditions));

        KeyTriggers = (KeyCondition[])keyConditions.Clone();

        Array.Sort(
            KeyTriggers,
            static (a, b) =>
            {
                int keyCodeComparison = a.Code.CompareTo(b.Code);

                if (keyCodeComparison != 0)
                    return keyCodeComparison;

                return a.State.CompareTo(b.State);
            }
        );
    }
}
