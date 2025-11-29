using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerVisual;

[HarmonyPatch(typeof(global::PlayerVisual), "Apply_NetworkedCharacterDisplay", MethodType.Normal)]
public static class Apply_NetworkedCharacterDisplay_Prefix
{
    public delegate void EventHandler(global::PlayerVisual PlayerVisual);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Prefix(global::PlayerVisual __instance) => OnInvoke?.Invoke(__instance);
#pragma warning restore IDE0051
}