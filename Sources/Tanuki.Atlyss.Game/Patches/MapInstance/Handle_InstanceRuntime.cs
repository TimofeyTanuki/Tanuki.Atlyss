using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), "Handle_InstanceRuntime", MethodType.Normal)]
public sealed class Handle_InstanceRuntime
{
    private static Action<global::MapInstance>? onPostfix;

    public static event Action<global::MapInstance> OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Handle_InstanceRuntime>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void PostfixMethod(global::MapInstance __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}