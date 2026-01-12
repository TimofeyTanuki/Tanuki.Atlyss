using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.LoadSceneManager;

[HarmonyPatch(typeof(global::LoadSceneManager), "Init_LoadScreenDisable", MethodType.Normal)]
public class Init_LoadScreenDisable
{
    public delegate void Postfix();
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Init_LoadScreenDisable, Postfix>(ref _OnPostfix, value);
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