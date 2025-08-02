using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game;

public class Main
{
    private readonly Harmony Harmony;
    private readonly HashSet<Type> Patches;
    public static Main Instance { get; private set; } = null;
    private Main()
    {
        Harmony = new("Tanuki.Atlyss.Game");
        Patches = [];
    }
    public static void Initialize() =>
        Instance ??= new();

    public void Patch(Type Type)
    {
        if (Patches.Contains(Type))
            return;

        if (!(Type.IsSealed && Type.IsAbstract))
            throw new ArgumentException($"Patch class {Type.FullName} must be static.");

        if (!Type.IsDefined(typeof(HarmonyPatch), false))
            throw new ArgumentException($"Patch class {Type.FullName} must have HarmonyPatch attribute.");

        Harmony.CreateClassProcessor(Type).Patch();
        Patches.Add(Type);
    }
    public void Patch(params Type[] Types)
    {
        foreach (Type Type in Types)
            Patch(Type);
    }
}