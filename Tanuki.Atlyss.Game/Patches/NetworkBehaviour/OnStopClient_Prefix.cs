using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStopClient), MethodType.Normal)]
public static class OnStopClient_Prefix
{
    public delegate void EventHandler(Mirror.NetworkBehaviour NetworkBehaviour);
    public static event EventHandler OnInvoke;

    public static void Prefix(Mirror.NetworkBehaviour __instance) => OnInvoke?.Invoke(__instance);
}