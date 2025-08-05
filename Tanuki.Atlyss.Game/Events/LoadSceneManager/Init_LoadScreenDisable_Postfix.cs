using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.LoadSceneManager;

[HarmonyPatch(typeof(global::LoadSceneManager), "Init_LoadScreenDisable", MethodType.Normal)]
public static class Init_LoadScreenDisable_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

    internal static void Postfix() => OnInvoke?.Invoke();
}