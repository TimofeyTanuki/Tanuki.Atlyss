using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStartClient), MethodType.Normal)]
public static class OnStartClient_Postfix
{
    public delegate void EventHandler(Mirror.NetworkBehaviour NetworkBehaviour);
    public static event EventHandler OnInvoke;

    public static void Postfix(Mirror.NetworkBehaviour __instance) => OnInvoke?.Invoke(__instance);
}