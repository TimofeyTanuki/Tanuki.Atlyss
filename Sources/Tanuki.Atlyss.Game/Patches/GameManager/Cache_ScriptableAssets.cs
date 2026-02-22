using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.GameManager;

[HarmonyPatch(typeof(global::GameManager), "Cache_ScriptableAssets", MethodType.Normal)]
public sealed class Cache_ScriptableAssets
{
    private static Action? onPostfix;

    public static event Action OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Cache_ScriptableAssets>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix()
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke();
    }
}
