using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.GameWorldManager;

[HarmonyPatch(typeof(global::GameWorldManager), "Server_DayNightCycleRuntime", MethodType.Normal)]
public class Server_DayNightCycleRuntime
{
    public delegate void Postfix();
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Server_DayNightCycleRuntime, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void PostfixMethod()
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke();
    }
}