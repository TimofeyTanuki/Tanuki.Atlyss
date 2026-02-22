using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), "New_CooldownSlot", MethodType.Normal)]
public sealed class New_CooldownSlot
{
    public delegate void PrefixHandler(global::PlayerCasting instance, ref ScriptableSkill setSkill, ref bool runOriginal);

    private static PrefixHandler? onPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<New_CooldownSlot>())
                onPrefix += value;
        }
        remove => onPrefix -= value;
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool Prefix(global::PlayerCasting __instance, ref ScriptableSkill _setSkill)
    {
        if (onPrefix is null)
            return true;

        bool runOriginal = true;
        onPrefix.Invoke(__instance, ref _setSkill, ref runOriginal);
        return runOriginal;
    }
}
