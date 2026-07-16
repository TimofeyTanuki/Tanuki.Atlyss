using System;
using System.Collections.Generic;
using Tanuki.Atlyss.Core.Types.Managers.Hotkey;
using Tanuki.Atlyss.Shared.Extensions;
using UnityEngine;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Hotkey
{
    private readonly Components.Hotkey hotkeyComponent;

    internal Hotkey() =>
        hotkeyComponent = Components.Hotkey.GetOrCreate();

    public void Register(IReadOnlyList<KeyCondition> keyCombination, Action action)
    {
        if (keyCombination.Count == 0)
            return;

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        SortedSet<KeyCondition> sortedKeyConditions = [];

        foreach (KeyCondition keyCondition in keyCombination)
        {
            if (keyCondition.Code == KeyCode.None)
                continue;

            sortedKeyConditions.Add(keyCondition);
        }

        if (sortedKeyConditions.Count == 0)
            return;

        KeyCondition[] keyConditions = new KeyCondition[sortedKeyConditions.Count];
        sortedKeyConditions.CopyTo(keyConditions);

        for (int combinationDefinitionIndex = 0; combinationDefinitionIndex < hotkeyComponent.combinationDefinitions.Count; combinationDefinitionIndex++)
        {
            CombinationDefinition existingCombinationDefinition = hotkeyComponent.combinationDefinitions[combinationDefinitionIndex];

            if (!existingCombinationDefinition.KeyConditions.SequenceEquals(keyConditions))
                continue;

            existingCombinationDefinition.Actions.Add(action);
            return;
        }

        CombinationDefinition combinationDefinition = new(keyConditions);
        hotkeyComponent.combinationDefinitions.Add(combinationDefinition);
        combinationDefinition.Actions.Add(action);

        hotkeyComponent.Rebuild();
    }

    public void Deregister(Action action)
    {
        for (int combinationDefinitionIndex = 0; combinationDefinitionIndex < hotkeyComponent.combinationDefinitions.Count; combinationDefinitionIndex++)
        {
            CombinationDefinition combinationDefinition = hotkeyComponent.combinationDefinitions[combinationDefinitionIndex];

            if (combinationDefinition.Actions.Remove(action))
            {
                if (combinationDefinition.Actions.Count == 0)
                    hotkeyComponent.combinationDefinitions.RemoveAt(combinationDefinitionIndex);

                hotkeyComponent.Rebuild();
                return;
            }
        }
    }
}
