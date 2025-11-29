using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;


[HarmonyPatch(typeof(global::AtlyssNetworkManager), "OnStartClient", MethodType.Normal)]
public static class OnStartClient_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;
#pragma warning disable IDE0051
    private static void Postfix() => OnInvoke?.Invoke();
#pragma warning restore IDE0051
}