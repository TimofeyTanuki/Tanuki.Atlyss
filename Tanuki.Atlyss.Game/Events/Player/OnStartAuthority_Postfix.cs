using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.Player;

[HarmonyPatch(typeof(global::Player), "OnStartAuthority", MethodType.Normal)]
public static class OnStartAuthority_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix() => OnInvoke?.Invoke();
#pragma warning restore IDE0051
}