using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStopClient), MethodType.Normal)]
public sealed class OnStopClient
{
    private static Action<Mirror.NetworkBehaviour>? onPrefix;

    public static event Action<Mirror.NetworkBehaviour> OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<OnStopClient>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix]
    private static void Prefix(Mirror.NetworkBehaviour __instance)
    {
        if (onPrefix is null)
            return;

        onPrefix.Invoke(__instance);
    }
}
