using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), nameof(global::PlayerCasting.Cmd_InitSkill), MethodType.Normal)]
public static class Cmd_InitSkill_Postfix
{
    public delegate void EventHandler(global::PlayerCasting PlayerCasting);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(global::PlayerCasting __instance) =>
        OnInvoke?.Invoke(__instance);
#pragma warning restore IDE0051
}