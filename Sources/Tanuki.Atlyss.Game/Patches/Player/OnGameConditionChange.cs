using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), "OnGameConditionChange", MethodType.Normal)]
public sealed class OnGameConditionChange
{
    private static Action<global::Player, GameCondition, GameCondition>? onPostfix;

    public static event Action<global::Player, GameCondition, GameCondition> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<OnGameConditionChange>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix(global::Player __instance, GameCondition _oldCondition, GameCondition _newCondition)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance, _oldCondition, _newCondition);
    }
}
