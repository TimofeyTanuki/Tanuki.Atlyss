using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), "Handle_InstanceRuntime", MethodType.Normal)]
public static class Handle_InstanceRuntime_Prefix
{
    public delegate void EventHandler(global::MapInstance MapInstance, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static bool Prefix(global::MapInstance __instance)
    {
        bool ShouldAllow = true;
        OnInvoke?.Invoke(__instance, ref ShouldAllow);

        return ShouldAllow;
    }
#pragma warning restore IDE0051
}