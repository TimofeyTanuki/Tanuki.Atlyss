using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.GameWorldManager;

[HarmonyPatch(typeof(global::GameWorldManager), "Awake", MethodType.Normal)]
public sealed class Awake
{
    private static Action<global::GameWorldManager>? onPostfix;

    public static event Action<global::GameWorldManager> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Awake>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix(global::GameWorldManager __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
