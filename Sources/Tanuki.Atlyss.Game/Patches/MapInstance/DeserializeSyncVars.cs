using HarmonyLib;
using Mirror;
using System;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), nameof(global::MapInstance.DeserializeSyncVars), MethodType.Normal)]
public sealed class DeserializeSyncVars
{
    private static Action<global::MapInstance, NetworkReader, bool, int>? onPrefix;
    private static Action<global::MapInstance, NetworkReader, bool, int>? onPostfix;

    public static event Action<global::MapInstance, NetworkReader, bool, int> OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<DeserializeSyncVars>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    public static event Action<global::MapInstance, NetworkReader, bool, int> OnPostfix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<DeserializeSyncVars>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPrefix]
    private static void Prefix(global::MapInstance __instance, NetworkReader reader, bool initialState, out int __state)
    {
        __state = reader.Position;

        if (onPrefix is null)
            return;

        onPrefix.Invoke(__instance, reader, initialState, __state);

        reader.Position = __state;
    }

    [HarmonyPostfix]
    private static void Postfix(global::MapInstance __instance, NetworkReader reader, bool initialState, int __state)
    {
        if (onPostfix is null)
            return;

        int originalPosition = reader.Position;
        onPostfix.Invoke(__instance, reader, initialState, __state);
        reader.Position = originalPosition;
    }
}