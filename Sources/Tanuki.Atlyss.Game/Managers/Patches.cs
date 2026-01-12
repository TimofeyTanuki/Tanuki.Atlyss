using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Managers;

internal class Patches
{
    private static readonly Harmony Harmony = new("97a2ac6772114ac9bfac3f83f85910e7");
    private static readonly ManualLogSource ManualLogSource = new("Tanuki.Atlyss.Game.Managers.Patches");

    private static readonly HashSet<Type> AppliedPatches = [];

    public static bool EnsurePatched<T>()
    {
        Type Type = typeof(T);

        if (AppliedPatches.Contains(Type))
            return true;

        try
        {
            Harmony.CreateClassProcessor(Type).Patch();
        }
        catch (Exception Exception)
        {
            ManualLogSource.LogError($"Failed to apply patch \"{Type.FullName}\".\nException:\n{Exception}");
            return false;
        }

        AppliedPatches.Add(Type);

        return true;
    }

    public static void Subscribe<TPatch, TDelegate>(ref TDelegate? Delegate, TDelegate Subscriber)
        where TPatch : class
        where TDelegate : Delegate
    {
        if (Subscriber is null)
            return;

        if (!EnsurePatched<TPatch>())
            return;

        Delegate = (TDelegate)System.Delegate.Combine(Delegate, Subscriber);
    }

    public static void Unsubscribe<TDelegate>(ref TDelegate? Delegate, TDelegate Subscriber)
        where TDelegate : Delegate
    {
        if (Subscriber is null ||
            Delegate is null)
            return;

        Delegate = (TDelegate)System.Delegate.Remove(Delegate, Subscriber);
    }
}