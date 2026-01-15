using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using Tanuki.Atlyss.API.Commands.Callers;

namespace Tanuki.Atlyss.Core.Managers;

public sealed class Chat
{
    internal Chat() { }

    public void OnPlayerChatted(string Message, ref bool ShouldAllow)
    {

    }

    public void SendClientMessage(string Message) =>
        Player._mainPlayer._chatBehaviour.New_ChatMessage(Message);

    public void SendServerMessage(Player Player, string Message) =>
        Player._chatBehaviour.Target_RecieveMessage(Message);
}