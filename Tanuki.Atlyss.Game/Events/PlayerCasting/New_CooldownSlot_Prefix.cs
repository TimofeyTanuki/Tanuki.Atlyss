using HarmonyLib;

namespace Tanuki.Atlyss.Game.Events.PlayerCasting;

[HarmonyPatch(typeof(global::PlayerCasting), "New_CooldownSlot", MethodType.Normal)]
public static class New_CooldownSlot_Prefix
{
    public delegate void EventHandler(global::PlayerCasting PlayerCasting, ref ScriptableSkill ScriptableSkill, ref bool ShouldAllow);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static bool Prefix(global::PlayerCasting __instance, ref ScriptableSkill _setSkill)
    {
        bool ShouldAllow = true;
        OnInvoke?.Invoke(__instance, ref _setSkill, ref ShouldAllow);
        return ShouldAllow;
    }
#pragma warning restore IDE0051
}