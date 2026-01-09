using HarmonyLib;
using Mirror;
using System;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), nameof(global::MapInstance.DeserializeSyncVars), MethodType.Normal)]
public static class DeserializeSyncVars
{
    public delegate void EventHandler(global::MapInstance MapInstance, NetworkReader NetworkReader, bool InitialState);
    public static event EventHandler OnPrefix;
    public static event EventHandler OnPostfix;

#pragma warning disable IDE0051
    private static void Prefix(global::MapInstance __instance, NetworkReader reader, bool initialState, out int __state)
    {
        __state = reader.Position;

        Delegate[] Delegates = OnPrefix?.GetInvocationList();
        if (Delegates is null)
            return;

        foreach (Delegate Delegate in OnPrefix.GetInvocationList())
        {
            ((EventHandler)Delegate)(__instance, reader, initialState);
            reader.Position = __state;
        }
    }

    private static void Postfix(global::MapInstance __instance, NetworkReader reader, bool initialState, int __state)
    {
        Delegate[] Delegates = OnPostfix?.GetInvocationList();
        if (Delegates is null)
            return;

        foreach (Delegate Delegate in OnPostfix.GetInvocationList())
        {
            reader.Position = __state;
            ((EventHandler)Delegate)(__instance, reader, initialState);
        }
    }
#pragma warning restore IDE0051
}