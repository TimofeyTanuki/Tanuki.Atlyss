using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.AtlyssNetworkManager;


[HarmonyPatch(typeof(global::AtlyssNetworkManager), "OnStopClient", MethodType.Normal)]
public static class OnStopClient_Prefix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

    public static void Prefix() => OnInvoke?.Invoke();
}