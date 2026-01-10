using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.StatusEntity;

[HarmonyPatch(typeof(global::StatusEntity), nameof(global::StatusEntity.Take_Damage), MethodType.Normal)]
public static class Take_Damage_Prefix
{
    public delegate void EventHandler(global::StatusEntity StatusEntity, ref DamageStruct _dmgStruct, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static bool Prefix(global::StatusEntity __instance, ref DamageStruct _dmgStruct)
    {
        bool ShouldAllow = true;
        OnInvoke?.Invoke(__instance, ref _dmgStruct, ref ShouldAllow);
        return ShouldAllow;
    }
#pragma warning restore IDE0051
}