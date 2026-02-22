using HarmonyLib;
using Mirror;
using System;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.SerializeSyncVars), MethodType.Normal)]
public sealed class SerializeSyncVars
{
    private static Action<global::Player, NetworkWriter, bool>? onPostfix;

    public static event Action<global::Player, NetworkWriter, bool> OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<SerializeSyncVars>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix]
    private static void Postfix(global::Player __instance, NetworkWriter writer, bool forceAll)
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke(__instance, writer, forceAll);
    }
}
