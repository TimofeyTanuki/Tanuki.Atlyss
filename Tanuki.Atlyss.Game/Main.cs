using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game;

public class Main
{
    public static Main Instance;

    private readonly Harmony Harmony;
    private readonly ManualLogSource ManualLogSource;
    private readonly HashSet<Type> Patches;

    private Main()
    {
        Harmony = new("Tanuki.Atlyss.Game");
        ManualLogSource = new("Tanuki.Atlyss.Game");
        Patches = [];
    }
    public static void Initialize() =>
        Instance ??= new();

    public void Patch(Type Type)
    {
        if (Patches.Contains(Type))
            return;

        if (!(Type.IsSealed && Type.IsAbstract))
        {
            ManualLogSource.LogError($"Patch class {Type.FullName} must be static.");
            return;
        }

        if (!Type.IsDefined(typeof(HarmonyPatch), false))
        {
            ManualLogSource.LogError($"Patch class {Type.FullName} must have HarmonyPatch attribute.");
            return;
        }

        Harmony.CreateClassProcessor(Type).Patch();
        Patches.Add(Type);
    }
    public void Patch(params Type[] Types)
    {
        foreach (Type Type in Types)
            Patch(Type);
    }
}