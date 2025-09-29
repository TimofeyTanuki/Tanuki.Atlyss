﻿using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), "Client_LocalPlayerControl", MethodType.Normal)]
public static class Client_LocalPlayerControl_Prefix
{
    public delegate void EventHandler(global::PlayerMove PlayerMove, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

    public static bool ShouldAllow;

#pragma warning disable IDE0051
    private static bool Prefix(global::PlayerMove __instance)
    {
        bool ShouldAllow = true;
        OnInvoke?.Invoke(__instance, ref ShouldAllow);
        return ShouldAllow;
    }
#pragma warning restore IDE0051
}