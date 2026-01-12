using HarmonyLib;
using Mirror;

namespace Tanuki.Atlyss.Game.Patches.Player;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.DeserializeSyncVars), MethodType.Normal)]
public class DeserializeSyncVars
{
    public delegate void Prefix(global::Player MapInstance, NetworkReader NetworkReader, bool InitialState, int InitialPosition);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<DeserializeSyncVars, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    public delegate void Postfix(global::Player MapInstance, NetworkReader NetworkReader, bool InitialState, int InitialPosition);
    private static Postfix? _OnPostfix;

    public static event Postfix OnPostfix
    {
        add => Managers.Patches.Subscribe<DeserializeSyncVars, Postfix>(ref _OnPostfix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPostfix, value);
    }

    [HarmonyPrefix]
    private static void PrefixMethod(global::Player __instance, NetworkReader reader, bool initialState, out int __state)
    {
        __state = reader.Position;

        if (_OnPrefix is null)
            return;

        _OnPrefix.Invoke(__instance, reader, initialState, __state);

        reader.Position = __state;
    }

    [HarmonyPostfix]
    private static void PostfixMethod(global::Player __instance, NetworkReader reader, bool initialState, int __state)
    {
        if (_OnPostfix is null)
            return;

        int Position = reader.Position;
        _OnPostfix.Invoke(__instance, reader, initialState, __state);
        reader.Position = Position;
    }
}