using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), nameof(global::PlayerMove.Init_Jump), MethodType.Normal)]
public class Init_Jump
{
    public delegate void Postfix(global::PlayerMove PlayerMove, float Force, float ForwardForce, float GravityMultiply, bool UseAnimation);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Init_Jump, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix]
    private static void PostfixMethod(global::PlayerMove __instance, float _force, float _forwardForce, float _gravityMultiply, bool _useAnim)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance, _force, _forwardForce, _gravityMultiply, _useAnim);
    }
}