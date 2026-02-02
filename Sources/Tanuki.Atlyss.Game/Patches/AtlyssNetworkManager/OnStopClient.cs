using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;

[HarmonyPatch(typeof(global::AtlyssNetworkManager), nameof(global::AtlyssNetworkManager.OnStopClient), MethodType.Normal)]
public sealed class OnStopClient
{
    private static Action? onPrefix;
    private static Action? onPostfix;

    public static event Action OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<OnStopClient>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    public static event Action OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<OnStopClient>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPrefix]
    private static void Prefix()
    {
        if (onPrefix is null)
            return;

        onPrefix.Invoke();
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke();
    }
}
