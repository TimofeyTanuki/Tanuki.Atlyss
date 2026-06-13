using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Core.Types.Managers.Hotkey;

public sealed class CombinationDefinition(KeyCondition[] keyConditions)
{
    public readonly KeyCondition[] KeyConditions = keyConditions;
    public List<Action> Actions = new(1);

    public readonly int KeyConditionsLength = keyConditions.Length;
}