using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.ItemObject;

[HarmonyPatch(typeof(global::ItemObject), "Enable_GroundCheckToVelocityZero", MethodType.Normal)]
public sealed class Enable_GroundCheckToVelocityZero
{
    private static Action<global::ItemObject>? onPostfix;

    public static event Action<global::ItemObject> OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Enable_GroundCheckToVelocityZero>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix(global::ItemObject __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
