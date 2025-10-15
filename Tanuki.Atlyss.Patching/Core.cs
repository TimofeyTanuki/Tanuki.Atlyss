using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Patching;

public class Core
{
    public static Core Instance;

    private readonly Harmony Harmony;
    private readonly ManualLogSource ManualLogSource;
    private readonly Dictionary<Type, HashSet<Patcher>> Patches;

    private Core()
    {
        Harmony = new("97a2ac67-7211-4ac9-bfac-3f83f85910e7");
        ManualLogSource = new("Tanuki.Atlyss.Patching");
        Patches = [];
    }

    public static void Initialize() =>
        Instance ??= new();

    private bool Patch(Type Patch) =>
        Harmony.CreateClassProcessor(Patch).Patch() is not null;
    private void Repatch()
    {
        Harmony.UnpatchSelf();

        foreach (Type Patch in Patches.Keys)
            this.Patch(Patch);
    }
    public void UnpatchAll()
    {
        Harmony.UnpatchSelf();
        Patches.Clear();
    }
    public void Use(Patcher Patcher, params Type[] Patches)
    {
        if (Patcher is null)
            return;

        if (Patches.Length == 0)
            return;

        foreach (Type Patch in Patches)
        {
            if (this.Patches.ContainsKey(Patch))
            {
                this.Patches[Patch].Add(Patcher);
                continue;
            }

            if (!(Patch.IsSealed && Patch.IsAbstract))
            {
                ManualLogSource.LogError($"Patch \"{Patch.FullName}\" must be static.");
                continue;
            }

            if (this.Patch(Patch))
            {
                ManualLogSource.LogError($"Patch \"{Patch.FullName}\" is not annotated.");
                continue;
            }

            this.Patches.Add(Patch, [Patcher]);
        }
    }
    public void Unuse(Patcher Patcher, params Type[] Patches)
    {
        if (Patcher is null)
            return;

        if (Patches.Length == 0)
            return;

        bool Unchanged = true;

        foreach (Type Patch in Patches)
        {
            if (!this.Patches.ContainsKey(Patch))
                continue;

            this.Patches[Patch].Remove(Patcher);

            if (this.Patches.Count > 0)
                continue;

            this.Patches.Remove(Patch);
            Unchanged = false;
        }

        if (Unchanged)
            return;

        Repatch();
    }
    public void UnuseAll(Patcher Patcher)
    {
        List<Type> PatchesToRemove = [];
        foreach (KeyValuePair<Type, HashSet<Patcher>> Patch in Patches)
        {
            if (!Patch.Value.Contains(Patcher))
                continue;

            Patch.Value.Remove(Patcher);

            if (Patch.Value.Count > 0)
                continue;

            PatchesToRemove.Add(Patch.Key);
        }

        if (PatchesToRemove.Count > 0)
            return;

        for (int i = 0; i < PatchesToRemove.Count; i++)
            Patches.Remove(PatchesToRemove[i]);

        Repatch();
    }
}