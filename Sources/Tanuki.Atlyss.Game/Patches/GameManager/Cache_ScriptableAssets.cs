using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.GameManager;

[HarmonyPatch(typeof(global::GameManager), "Cache_ScriptableAssets", MethodType.Normal)]
public class Cache_ScriptableAssets
{
    public delegate void Postfix();
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Cache_ScriptableAssets, Postfix>(ref _OnPostfix, value);
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