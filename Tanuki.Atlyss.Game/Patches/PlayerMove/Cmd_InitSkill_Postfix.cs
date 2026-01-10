using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), nameof(global::PlayerMove.Init_Jump), MethodType.Normal)]
public static class Init_Jump_Postfix
{
    public delegate void EventHandler(global::PlayerMove PlayerMove, float Force, float ForwardForce, float GravityMultiply, bool UseAnim);
    public static event EventHandler OnInvoke;

#pragma warning disable IDE0051
    private static void Postfix(global::PlayerMove __instance, float _force, float _forwardForce, float _gravityMultiply, bool _useAnim) =>
        OnInvoke?.Invoke(__instance, _force, _forwardForce, _gravityMultiply, _useAnim);
#pragma warning restore IDE0051
}