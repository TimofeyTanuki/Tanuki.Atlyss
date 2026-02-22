using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Tanuki.Atlyss.Game.Patches.SteamLobby;

[HarmonyPatch(typeof(global::SteamLobby), "Reset_LobbyQueueParams", MethodType.Normal)]
public sealed class Reset_LobbyQueueParams
{
    private static Action? onPostfix;

    public static event Action OnPostfix
    {
        add
        {
            if (Utilities.Patches.EnsurePatched<Reset_LobbyQueueParams>())
                onPostfix += value;
        }
        remove => onPostfix -= value;
    }

    [HarmonyPostfix, SuppressMessage("CodeQuality", "IDE0051")]
    private static void Postfix()
    {
        if (onPostfix is null)
            return;

        onPostfix.Invoke();
    }
}
