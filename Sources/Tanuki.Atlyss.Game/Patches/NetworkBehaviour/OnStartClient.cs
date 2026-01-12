using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStartClient), MethodType.Normal)]
public class OnStartClient
{
    public delegate void Postfix(Mirror.NetworkBehaviour NetworkBehaviour);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<OnStartClient, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix]
    private static void PostfixMethod(Mirror.NetworkBehaviour __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}