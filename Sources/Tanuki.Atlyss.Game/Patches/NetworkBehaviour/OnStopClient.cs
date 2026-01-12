using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.NetworkBehaviour;

[HarmonyPatch(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.OnStopClient), MethodType.Normal)]
public class OnStopClient
{
    public delegate void Prefix(Mirror.NetworkBehaviour NetworkBehaviour);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<OnStopClient, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static void PrefixMethod(Mirror.NetworkBehaviour __instance)
    {
        if (_OnPrefix is null)
            return;

        _OnPrefix.Invoke(__instance);
    }
}