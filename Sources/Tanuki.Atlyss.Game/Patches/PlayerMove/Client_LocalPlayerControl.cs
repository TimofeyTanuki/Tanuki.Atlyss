using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.PlayerMove;

[HarmonyPatch(typeof(global::PlayerMove), "Client_LocalPlayerControl", MethodType.Normal)]
public class Client_LocalPlayerControl
{
    public delegate void Prefix(global::PlayerMove PlayerMove, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<Client_LocalPlayerControl, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix, SuppressMessage("CodeQuality", "IDE0051")]
    private static bool PrefixMethod(global::PlayerMove __instance)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix?.Invoke(__instance, ref ShouldAllow);
        return ShouldAllow;
    }
}