using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.OnStartAuthority), MethodType.Normal)]
public class OnStartAuthority
{
    public delegate void Postfix(global::Player Player);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<OnStartAuthority, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix]
    private static void PostfixMethod(global::Player __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}