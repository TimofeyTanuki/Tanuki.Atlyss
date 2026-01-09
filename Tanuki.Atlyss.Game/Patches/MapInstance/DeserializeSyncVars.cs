using HarmonyLib;
using Mirror;
using System;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), nameof(global::MapInstance.DeserializeSyncVars), MethodType.Normal)]
public static class DeserializeSyncVars
{
    private struct State(int Position)
    {
        public int Position = Position;
        public bool Cancelled = false;
    }

    public delegate void Before(global::MapInstance MapInstance, NetworkReader NetworkReader, bool InitialState, ref bool ShouldAllow);
    public static event Before OnBefore;

    public delegate void After(global::MapInstance MapInstance, NetworkReader NetworkReader, bool InitialState, bool Cancelled);
    public static event After OnAfter;

#pragma warning disable IDE0051
    private static bool Prefix(global::MapInstance __instance, NetworkReader reader, bool initialState, out State __state)
    {
        int Position = reader.Position;
        __state = new(Position);

        Delegate[] Delegates = OnBefore?.GetInvocationList();
        if (Delegates is null)
            return true;

        bool ShouldAllow = true;

        foreach (Delegate Delegate in OnBefore.GetInvocationList())
        {
            ((Before)Delegate)(__instance, reader, initialState, ref ShouldAllow);
            reader.Position = Position;
        }

        __state.Cancelled = !ShouldAllow;

        return ShouldAllow;
    }

    private static void Postfix(global::MapInstance __instance, NetworkReader reader, bool initialState, State __state)
    {
        Delegate[] Delegates = OnAfter?.GetInvocationList();
        if (Delegates is null)
            return;

        foreach (Delegate Delegate in OnAfter.GetInvocationList())
        {
            reader.Position = __state.Position;
            ((After)Delegate)(__instance, reader, initialState, __state.Cancelled);
        }
    }
#pragma warning restore IDE0051
}