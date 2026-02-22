using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), "Awake", MethodType.Normal)]
public sealed class Awake
{
    private static Action<global::Player>? onPostfix;

    public static event Action<global::Player> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Awake>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix(global::Player __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
