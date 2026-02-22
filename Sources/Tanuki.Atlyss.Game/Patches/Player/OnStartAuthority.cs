using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.OnStartAuthority), MethodType.Normal)]
public sealed class OnStartAuthority
{
    private static Action<global::Player>? onPostfix;

    public static event Action<global::Player> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<OnStartAuthority>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix(global::Player __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
