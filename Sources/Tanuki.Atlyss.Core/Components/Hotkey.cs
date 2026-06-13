using BepInEx;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tanuki.Atlyss.Core.Types.Managers.Hotkey;
using Tanuki.Atlyss.Shared.Extensions;
using UnityEngine;

namespace Tanuki.Atlyss.Core.Components;

internal sealed class Hotkey() : MonoBehaviour
{
    private const int DEFAULT_COLLECTION_CAPACITY = 4;

    private static Hotkey instance = null!;

    public static Hotkey Instance => instance;

    internal IInputSystem inputSystem = null!;
    internal List<CombinationDefinition> combinationDefinitions = [];


    private readonly Dictionary<KeyCondition, List<int>> combinationMap = [];

    private CombinationRuntime[] combinationRuntimes = [];
    private bool[] activeMask = new bool[DEFAULT_COLLECTION_CAPACITY];

    private int[]
        combinationMatches = new int[DEFAULT_COLLECTION_CAPACITY],
        combinationTouches = new int[DEFAULT_COLLECTION_CAPACITY],
        activeCombinations = new int[DEFAULT_COLLECTION_CAPACITY],
        finalCombinations = new int[DEFAULT_COLLECTION_CAPACITY];

    private int
        activeCombinationsCount,
        currentCombinationTouches,
        currentCombinationsCount;

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Awake()
    {
        if (instance is not null && instance != this)
        {
            instance = this;
            return;
        }

        instance = this;

        enabled = false;
        DontDestroyOnLoad(this);
    }

    internal void Rebuild()
    {
        combinationMap.Clear();

        int definedCombinations = combinationDefinitions.Count;

        if (combinationMatches.Length < definedCombinations)
            Array.Resize(ref combinationMatches, definedCombinations);

        if (combinationTouches.Length < definedCombinations)
            Array.Resize(ref combinationTouches, definedCombinations);

        if (activeCombinations.Length < definedCombinations)
            Array.Resize(ref activeCombinations, definedCombinations);

        if (finalCombinations.Length < definedCombinations)
            Array.Resize(ref finalCombinations, definedCombinations);

        if (activeMask.Length < definedCombinations)
            Array.Resize(ref activeMask, definedCombinations);

        for (int definedCombinationIndex = 0; definedCombinationIndex < definedCombinations; definedCombinationIndex++)
        {
            CombinationDefinition combinationDefinition = combinationDefinitions[definedCombinationIndex];

            for (int keyConditionIndex = 0; keyConditionIndex < combinationDefinition.KeyConditions.Length; keyConditionIndex++)
            {
                KeyCondition keyCondition = combinationDefinition.KeyConditions[keyConditionIndex];

                if (!combinationMap.TryGetValue(keyCondition, out var combinationIndices))
                {
                    combinationIndices = new(2);
                    combinationMap.Add(keyCondition, combinationIndices);
                }

                combinationIndices.Add(definedCombinationIndex);
            }
        }

        combinationRuntimes = new CombinationRuntime[combinationMap.Count];

        int combinationRuntimeIndex = 0;
        foreach (KeyValuePair<KeyCondition, List<int>> combinations in combinationMap)
            combinationRuntimes[combinationRuntimeIndex++] = new(combinations.Key, [.. combinations.Value]);

        enabled = combinationMap.Count > 0;
    }

    private void FindActiveCombinations()
    {
        activeCombinationsCount = 0;
        currentCombinationTouches = 0;

        for (int combinationRuntimeIndex = 0; combinationRuntimeIndex < combinationRuntimes.Length; combinationRuntimeIndex++)
        {
            ref readonly CombinationRuntime combinationRuntime = ref combinationRuntimes[combinationRuntimeIndex];

            bool active = combinationRuntime.KeyCondition.State switch
            {
                EKeyState.Pressed => inputSystem.GetKeyDown(combinationRuntime.KeyCondition.Code),
                EKeyState.Held => inputSystem.GetKey(combinationRuntime.KeyCondition.Code),
                EKeyState.Released => inputSystem.GetKeyUp(combinationRuntime.KeyCondition.Code),
                _ => false
            };

            if (!active)
                continue;

            int[] combinationIds = combinationRuntime.CombinationIds;

            for (int combinationIdIndex = 0; combinationIdIndex < combinationIds.Length; combinationIdIndex++)
            {
                int combinationId = combinationIds[combinationIdIndex];

                if (combinationMatches[combinationId]++ == 0)
                    combinationTouches[currentCombinationTouches++] = combinationId;

                if (combinationMatches[combinationId] == combinationDefinitions[combinationId].KeyConditions.Length)
                    activeCombinations[activeCombinationsCount++] = combinationId;
            }
        }
    }

    private void ResolveDominance()
    {
        currentCombinationsCount = 0;

        for (int activeCombinationIndex = 0; activeCombinationIndex < activeCombinationsCount; activeCombinationIndex++)
            activeMask[activeCombinations[activeCombinationIndex]] = true;

        for (int currentCombinationIndex = 0; currentCombinationIndex < activeCombinationsCount; currentCombinationIndex++)
        {
            int currentCombinationId = activeCombinations[currentCombinationIndex];
            CombinationDefinition currentCombinationDefinition = combinationDefinitions[currentCombinationId];

            bool dominated = false;

            for (int otherCombinationIndex = 0; otherCombinationIndex < activeCombinationsCount; otherCombinationIndex++)
            {
                int otherCombinationId = activeCombinations[otherCombinationIndex];

                if (otherCombinationId == currentCombinationId)
                    continue;

                CombinationDefinition otherCombinationDefinition = combinationDefinitions[otherCombinationId];

                if (otherCombinationDefinition.KeyConditions.Length > currentCombinationDefinition.KeyConditions.Length &&
                    otherCombinationDefinition.KeyConditions.ContainsAll(currentCombinationDefinition.KeyConditions))
                {
                    dominated = true;
                    break;
                }
            }

            if (!dominated)
                finalCombinations[currentCombinationsCount++] = currentCombinationId;
        }
    }

    private void ProcessActiveCombinations()
    {
        for (int combinationIndex = 0; combinationIndex < currentCombinationsCount; combinationIndex++)
        {
            int combinationId = finalCombinations[combinationIndex];

            List<Action> actions = combinationDefinitions[combinationId].Actions;

            for (int actionIndex = 0; actionIndex < actions.Count; actionIndex++)
                actions[actionIndex]();
        }
    }

    private void ResetActiveMatches()
    {
        for (int combinationTouchIndex = 0; combinationTouchIndex < currentCombinationTouches; combinationTouchIndex++)
            combinationMatches[combinationTouches[combinationTouchIndex]] = 0;

        for (int combinationIndex = 0; combinationIndex < activeCombinationsCount; combinationIndex++)
            activeMask[activeCombinations[combinationIndex]] = false;
    }

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Update()
    {
        FindActiveCombinations();
        ResolveDominance();
        ProcessActiveCombinations();
        ResetActiveMatches();
    }
}
