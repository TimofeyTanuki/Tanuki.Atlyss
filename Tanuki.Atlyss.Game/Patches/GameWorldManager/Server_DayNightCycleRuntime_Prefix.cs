using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.GameWorldManager;

[HarmonyPatch(typeof(global::GameWorldManager), "Server_DayNightCycleRuntime", MethodType.Normal)]
public static class Server_DayNightCycleRuntime_Prefix
{
    public delegate void EventHandler(ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static bool Prefix()
    {
        bool ShouldAllow = true;
        OnInvoke?.Invoke(ref ShouldAllow);

        return ShouldAllow;
    }
#pragma warning restore IDE0051
}