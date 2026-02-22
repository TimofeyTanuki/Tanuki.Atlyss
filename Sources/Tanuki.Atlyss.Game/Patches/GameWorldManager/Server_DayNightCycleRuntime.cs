using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.GameWorldManager;

[HarmonyPatch(typeof(global::GameWorldManager), "Server_DayNightCycleRuntime", MethodType.Normal)]
public sealed class Server_DayNightCycleRuntime
{
    private static Action? onPostfix;

    public static event Action OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Server_DayNightCycleRuntime>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix()
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke();
    }
}
