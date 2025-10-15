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
    private bool ApplyPatch(Type Type) =>
        Harmony.CreateClassProcessor(Type).Patch() is not null;
    public void Patch(params Type[] Types)
    {
        if (Types.Length == 0)
            return;

        foreach (Type Type in Types)
        {
            if (Patches.Contains(Type))
                continue;

            if (!(Type.IsSealed && Type.IsAbstract))
            {
                ManualLogSource.LogError($"ApplyPatch \"{Type.FullName}\" must be static.");
                continue;
            }

            if (ApplyPatch(Type))
            {
                ManualLogSource.LogError($"ApplyPatch \"{Type.FullName}\" is not annotated.");
                continue;
            }

            Patches.Add(Type);
        }
    }
    public void Unpatch(params Type[] Types)
    {
        if (Types.Length == 0)
            return;

        foreach (Type Type in Types)
            Patches.Remove(Type);

        Harmony.UnpatchSelf();

        foreach (Type Type in Patches)
            ApplyPatch(Type);
    }
}