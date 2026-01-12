using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ScriptablePlayerRace;

[HarmonyPatch(typeof(global::ScriptablePlayerRace), nameof(global::ScriptablePlayerRace.Init_ParamsCheck), MethodType.Normal)]
public class Init_ParamsCheck
{
    public delegate void Prefix(PlayerAppearance_Profile PlayerAppearance, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<Init_ParamsCheck, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static bool PrefixMethod(PlayerAppearance_Profile _aP, ref PlayerAppearance_Profile __result)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix.Invoke(_aP, ref ShouldAllow);

        if (ShouldAllow)
            return true;

        __result = _aP;
        return ShouldAllow;
    }
}