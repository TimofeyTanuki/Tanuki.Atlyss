using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), "Awake", MethodType.Normal)]
public class Awake
{
    public delegate void Postfix(global::Player Player);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Awake, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void PostfixMethod(global::Player __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}