/*
using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), "OnServerMessage", MethodType.Normal)]
public static class OnServerMessage_Postfix
{
    public delegate void EventHandler(ServerMessage Message);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(ServerMessage _message) => OnInvoke?.Invoke(_message);
#pragma warning restore IDE0051
}
*/