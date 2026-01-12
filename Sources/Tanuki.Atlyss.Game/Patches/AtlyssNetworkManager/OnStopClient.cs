using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.AtlyssNetworkManager;

[HarmonyPatch(typeof(global::AtlyssNetworkManager), nameof(global::AtlyssNetworkManager.OnStopClient), MethodType.Normal)]
public class OnStopClient
{
    public delegate void Prefix();
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<OnStopClient, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static void PrefixMethod()
    {
        if (_OnPrefix is null)
            return;

        _OnPrefix.Invoke();
    }
}