using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ScriptablePlayerRace;

[HarmonyPatch(typeof(global::ScriptablePlayerRace), nameof(global::ScriptablePlayerRace.Init_ParamsCheck), MethodType.Normal)]
public sealed class Init_ParamsCheck
{
    public delegate void PrefixHandler(PlayerAppearance_Profile appearanceProfile, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Init_ParamsCheck>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix]
    private static bool Prefix(PlayerAppearance_Profile _aP, ref PlayerAppearance_Profile __result)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;

        onPrefix.Invoke(_aP, ref runOriginal);

        if (runOriginal)
            return true;

        __result = _aP;
        return runOriginal;
    }
}
