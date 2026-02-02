using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.Network_steamID), MethodType.Setter)]
public sealed class Network_steamID_Setter
{
    private static Action<global::Player>? onPostfix;

    public static event Action<global::Player> OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Network_steamID_Setter>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix(global::Player __instance)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance);
    }
}
