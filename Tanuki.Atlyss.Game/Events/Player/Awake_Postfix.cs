using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.Player;

[HarmonyPatch(typeof(global::Player), "Awake", MethodType.Normal)]
public static class Awake_Postfix
{
    public delegate void EventHandler(global::Player Player);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(global::Player __instance) => OnInvoke?.Invoke(__instance);
#pragma warning restore IDE0051
}