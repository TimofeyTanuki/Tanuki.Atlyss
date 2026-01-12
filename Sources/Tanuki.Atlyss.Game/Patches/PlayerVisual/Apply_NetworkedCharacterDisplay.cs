using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerVisual;

[HarmonyPatch(typeof(global::PlayerVisual), nameof(global::PlayerVisual.Apply_NetworkedCharacterDisplay), MethodType.Normal)]
public class Apply_NetworkedCharacterDisplay
{
    public delegate void Prefix(global::PlayerVisual PlayerVisual, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<Apply_NetworkedCharacterDisplay, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static bool PrefixMethod(global::PlayerVisual __instance)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix.Invoke(__instance, ref ShouldAllow);
        return ShouldAllow;
    }
}