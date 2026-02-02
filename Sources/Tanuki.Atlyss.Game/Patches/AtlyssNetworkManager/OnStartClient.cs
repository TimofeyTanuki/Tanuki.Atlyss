using HarmonyLib;
using System;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;

[HarmonyPatch(typeof(global::AtlyssNetworkManager), nameof(global::AtlyssNetworkManager.OnStartClient), MethodType.Normal)]
public sealed class OnStartClient
{
    private static Action? onPostfix;

    public static event Action OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<OnStartClient>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke();
    }
}
