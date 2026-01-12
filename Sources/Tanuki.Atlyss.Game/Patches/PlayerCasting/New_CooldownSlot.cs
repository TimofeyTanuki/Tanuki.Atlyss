using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), "New_CooldownSlot", MethodType.Normal)]
public class New_CooldownSlot
{
    public delegate void Prefix(global::PlayerCasting PlayerCasting, ref ScriptableSkill ScriptableSkill, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<New_CooldownSlot, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool PrefixMethod(global::PlayerCasting __instance, ref ScriptableSkill _setSkill)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix.Invoke(__instance, ref _setSkill, ref ShouldAllow);
        return ShouldAllow;
    }
}