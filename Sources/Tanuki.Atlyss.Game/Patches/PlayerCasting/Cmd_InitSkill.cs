using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), nameof(global::PlayerCasting.Cmd_InitSkill), MethodType.Normal)]
public sealed class Cmd_InitSkill
{
    private static Action<global::PlayerCasting>? onPostfix;

    public static event Action<global::PlayerCasting> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Cmd_InitSkill>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix(global::PlayerCasting __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
