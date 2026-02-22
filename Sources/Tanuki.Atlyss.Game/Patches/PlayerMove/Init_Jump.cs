using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), nameof(global::PlayerMove.Init_Jump), MethodType.Normal)]
public sealed class Init_Jump
{
    private static Action<global::PlayerMove, float, float, float, bool>? onPostfix;

    public static event Action<global::PlayerMove, float, float, float, bool> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Init_Jump>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix(global::PlayerMove __instance, float _force, float _forwardForce, float _gravityMultiply, bool _useAnim)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance, _force, _forwardForce, _gravityMultiply, _useAnim);
    }
}
