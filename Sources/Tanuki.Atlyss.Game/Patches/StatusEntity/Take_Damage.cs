using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.StatusEntity;

[HarmonyPatch(typeof(global::StatusEntity), nameof(global::StatusEntity.Take_Damage), MethodType.Normal)]
public class Take_Damage
{
    public delegate void Prefix(global::StatusEntity StatusEntity, ref DamageStruct _dmgStruct, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<Take_Damage, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static bool PrefixMethod(global::StatusEntity __instance, ref DamageStruct _dmgStruct)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix.Invoke(__instance, ref _dmgStruct, ref ShouldAllow);
        return ShouldAllow;
    }
}