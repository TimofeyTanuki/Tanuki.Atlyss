using HarmonyLib;
using Mirror;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), "DeserializeSyncVars", MethodType.Normal)]
public static class DeserializeSyncVars_Postfix
{
    public delegate void EventHandler(global::Player Player, NetworkReader NetworkReader, bool InitialState);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(global::Player __instance, NetworkReader reader, bool initialState) => OnInvoke?.Invoke(__instance, reader, initialState);
#pragma warning restore IDE0051
}