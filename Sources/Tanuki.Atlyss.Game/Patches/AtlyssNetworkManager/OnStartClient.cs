using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;

[HarmonyPatch(typeof(global::AtlyssNetworkManager), nameof(global::AtlyssNetworkManager.OnStartClient), MethodType.Normal)]
public class OnStartClient
{
    public delegate void Postfix();
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<OnStartClient, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix]
    private static void PostfixMethod()
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke();
    }
}