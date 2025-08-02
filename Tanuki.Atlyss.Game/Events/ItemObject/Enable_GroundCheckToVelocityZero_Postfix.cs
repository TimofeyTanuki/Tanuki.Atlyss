using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.ItemObject;

[HarmonyPatch(typeof(global::ItemObject), "Enable_GroundCheckToVelocityZero", MethodType.Normal)]
public static class Enable_GroundCheckToVelocityZero_Postfix
{
    public delegate void EventHandler(global::ItemObject ItemObject);
    public static event EventHandler OnInvoke;

    internal static void Postfix(global::ItemObject __instance) => OnInvoke?.Invoke(__instance);
}