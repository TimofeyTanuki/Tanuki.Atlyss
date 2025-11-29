using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.GameManager;


[HarmonyPatch(typeof(global::GameManager), "Cache_ScriptableAssets", MethodType.Normal)]
public static class Cache_ScriptableAssets_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix() => OnInvoke?.Invoke();
#pragma warning restore IDE0051
}