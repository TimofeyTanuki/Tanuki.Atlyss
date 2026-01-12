using HarmonyLib;
using Tanuki.Atlyss.Game.Patches.Player;

namespace Tanuki.Atlyss.Game.Patches.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), nameof(global::PlayerCasting.Cmd_InitSkill), MethodType.Normal)]
public class Cmd_InitSkill
{
    public delegate void Postfix(global::PlayerCasting PlayerCasting);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Awake, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix]
    private static void PostfixMethod(global::PlayerCasting __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}