using HarmonyLib;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Accessors;

public static class ChatBehaviour
{
    public static readonly AccessTools.FieldRef<global::ChatBehaviour, Player> _player;

    static ChatBehaviour() =>
        _player = AccessTools.FieldRefAccess<global::ChatBehaviour, Player>("_player");
}