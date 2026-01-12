using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.ItemObject;

[HarmonyPatch(typeof(global::ItemObject), "Enable_GroundCheckToVelocityZero", MethodType.Normal)]
public class Enable_GroundCheckToVelocityZero
{
    public delegate void Postfix(global::ItemObject ItemObject);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<Enable_GroundCheckToVelocityZero, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void PostfixMethod(global::ItemObject __instance)
    {
        if (_OnPostfix is null)
            return;

        _OnPostfix.Invoke(__instance);
    }
}