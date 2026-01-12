using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.MapInstance;

[HarmonyPatch(typeof(global::MapInstance), "Handle_InstanceRuntime", MethodType.Normal)]
public class Handle_InstanceRuntime
{
    public delegate void Postfix(global::MapInstance MapInstance);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Handle_InstanceRuntime, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void PostfixMethod(global::MapInstance __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}