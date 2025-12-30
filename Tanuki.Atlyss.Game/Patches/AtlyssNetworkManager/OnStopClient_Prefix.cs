using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;


[HarmonyPatch(typeof(global::AtlyssNetworkManager), "OnStopClient", MethodType.Normal)]
public static class OnStopClient_Prefix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;


#pragma warning disable IDE0051
    private static void Prefix() => OnInvoke?.Invoke();
#pragma warning restore IDE0051
}