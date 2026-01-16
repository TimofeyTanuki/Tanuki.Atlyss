using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStartClient), MethodType.Normal)]
public sealed class OnStartClient
{
    private static Action<Mirror.NetworkBehaviour>? onPostfix;

    public static event Action<Mirror.NetworkBehaviour> OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<OnStartClient>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix(Mirror.NetworkBehaviour __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}