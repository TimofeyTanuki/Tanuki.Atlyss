using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.StatusEntity;

[HarmonyPatch(typeof(global::StatusEntity), "Subtract_Health", MethodType.Normal)]
public static class Subtract_Health_Prefix
{
    public delegate void EventHandler(global::StatusEntity StatusEntity, ref int Value);
    public static event EventHandler OnInvoke;

    internal static void Prefix(global::StatusEntity __instance, ref int _value) =>
        OnInvoke?.Invoke(__instance, ref _value);
}