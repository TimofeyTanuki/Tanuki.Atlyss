﻿using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.ScriptablePlayerRace;

[HarmonyPatch(typeof(global::ScriptablePlayerRace), "Init_ParamsCheck", MethodType.Normal)]
public static class Init_ParamsCheck_Prefix
{
    public delegate void EventHandler(PlayerAppearance_Profile PlayerAppearance, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

    internal static bool Prefix(PlayerAppearance_Profile _aP, ref PlayerAppearance_Profile __result)
    {
        bool ShouldAllow = false;
        OnInvoke?.Invoke(_aP, ref ShouldAllow);

        if (!ShouldAllow)
            __result = _aP;

        return ShouldAllow;
    }
}