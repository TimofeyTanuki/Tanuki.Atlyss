using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.LoadSceneManager;

[HarmonyPatch(typeof(global::LoadSceneManager), "Init_LoadScreenDisable", MethodType.Normal)]
public static class Init_LoadScreenDisable_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix() => OnInvoke?.Invoke();
#pragma warning restore IDE0051
}