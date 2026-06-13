using BepInEx;
using System;
using System.Collections.Generic;
using Tanuki.Atlyss.Core.Types.Managers.Hotkey;
using Tanuki.Atlyss.Shared.Extensions;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Hotkey
{
    private readonly Components.Hotkey hotkeyComponent;

    internal Hotkey(IInputSystem inputSystem)
    {
        hotkeyComponent = Main.Instance.gameObject.AddComponent<Components.Hotkey>();
        hotkeyComponent.inputSystem = inputSystem;
    }

    public void Register(IReadOnlyList<KeyCondition> keyCombination, Action action)
    {
        if (keyCombination.Count == 0)
            throw new ArgumentException("At least one key condition must be provided.", nameof(keyCombination));

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        KeyCondition[] keyConditions = [.. keyCombination];

        Array.Sort(keyConditions);

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
