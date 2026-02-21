using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Managers;

public sealed class Patches
{
    private static readonly Harmony harmony = new("97a2ac6772114ac9bfac3f83f85910e7");
    private static readonly ManualLogSource manualLogSource = new("Tanuki.Atlyss.Game");
    private static readonly HashSet<Type> appliedPatches = [];

    internal Patches() { }

    public static bool EnsurePatched<T>()
    {
        Type patchType = typeof(T);

        if (appliedPatches.Contains(patchType))
            return true;

        try
        {
            harmony.CreateClassProcessor(patchType).Patch();
        }
        catch (Exception Exception)
        {
            manualLogSource.LogError($"Failed to apply patch {patchType.FullName}.\nException:\n{Exception.Message}\nStack trace:\n{Exception.StackTrace}");
            return false;
        }

        appliedPatches.Add(patchType);

        return true;
    }
}
