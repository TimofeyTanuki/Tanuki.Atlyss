using HarmonyLib;

namespace Tanuki.Atlyss.Game.Patches.ChatBehaviour;

[HarmonyPatch(typeof(global::ChatBehaviour), nameof(global::ChatBehaviour.Send_ChatMessage), MethodType.Normal)]
public class Send_ChatMessage
{
    public delegate void Prefix(string Message, ref bool ShouldAllow);
    private static Prefix? _OnPrefix;

    public static event Prefix OnPrefix
    {
        add => Managers.Patches.Subscribe<Send_ChatMessage, Prefix>(ref _OnPrefix, value);
        remove => Managers.Patches.Unsubscribe(ref _OnPrefix, value);
    }

    [HarmonyPrefix]
    private static bool PrefixMethod(string _message)
    {
        if (_OnPrefix is null)
            return true;

        bool ShouldAllow = true;
        _OnPrefix.Invoke(_message, ref ShouldAllow);
        return ShouldAllow;
    }
}