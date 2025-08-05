using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.GameManager;


[HarmonyPatch(typeof(global::GameManager), "Cache_ScriptableAssets", MethodType.Normal)]
public static class Cache_ScriptableAssets_Postfix
{
    public delegate void EventHandler();
    public static event EventHandler OnInvoke;

    internal static void Postfix() => OnInvoke?.Invoke();
}