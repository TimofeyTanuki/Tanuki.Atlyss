using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.LoadSceneManager;

[HarmonyPatch(typeof(global::LoadSceneManager), "Init_LoadScreenDisable", MethodType.Normal)]
public sealed class Init_LoadScreenDisable
{
    private static Action? onPostfix;

    public static event Action OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Init_LoadScreenDisable>())
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
