using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.AtlyssNetworkManager;


[HarmonyPatch(typeof(global::AtlyssNetworkManager), "OnStartClient", MethodType.Normal)]
public static class OnStartClient_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

    internal static void Postfix() => OnInvoke?.Invoke();
}