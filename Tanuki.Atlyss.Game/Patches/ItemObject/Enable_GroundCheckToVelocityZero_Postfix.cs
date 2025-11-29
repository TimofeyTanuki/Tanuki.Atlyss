using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ItemObject;

[HarmonyPatch(typeof(global::ItemObject), "Enable_GroundCheckToVelocityZero", MethodType.Normal)]
public static class Enable_GroundCheckToVelocityZero_Postfix
{
    public delegate void EventHandler(global::ItemObject ItemObject);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(global::ItemObject __instance) => OnInvoke?.Invoke(__instance);
#pragma warning restore IDE0051
}