using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.StatusEntity;

[HarmonyPatch(typeof(global::StatusEntity), nameof(global::StatusEntity.Take_Damage), MethodType.Normal)]
public sealed class Take_Damage
{
    public delegate void PrefixHandler(global::StatusEntity StatusEntity, ref DamageStruct damageStruct, ref bool runOriginal);

    private static PrefixHandler? _OnPrefix;

    public static event PrefixHandler OnPrefix
    {
        add
        {
            if (Managers.Patches.EnsurePatched<Take_Damage>())
                _OnPrefix += value;
        }
        remove => _OnPrefix -= value;
    }

    [HarmonyPrefix]
    private static bool Prefix(global::StatusEntity __instance, ref DamageStruct _dmgStruct)
    {
        if (_OnPrefix is null)
            return true;

        bool runOriginal = true;
        _OnPrefix.Invoke(__instance, ref _dmgStruct, ref runOriginal);
        return runOriginal;
    }
}