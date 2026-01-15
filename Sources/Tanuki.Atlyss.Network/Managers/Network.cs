using Steamworks;
using System;
using System.Text;
using Tanuki.Atlyss.Network.Components;

namespace Tanuki.Atlyss.Network.Managers;

public sealed class Network
{
    private const ushort LOBBY_CHATMESSAGE_BUFFER = 4096;

    public static Network Instance = null!;
    private bool IsSteamInitialized = false;
    private SteamNetworkMessagesPoller? SteamNetworkMessagesPoller;

    internal Network()
    {
        Steam Steam = Tanuki.Instance.Steam;


    }

    private void OnSteamRelayNetworkStatus(SteamRelayNetworkStatus_t SteamRelayNetworkStatus)
    {
        Console.WriteLine($"OnSteamRelayNetworkStatus {SteamRelayNetworkStatus.m_eAvail}");
        if (IsSteamInitialized)
            return;

        IsSteamInitialized = SteamRelayNetworkStatus.m_eAvail == ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;

        if (!IsSteamInitialized)
            return;

        SteamNetworkingUtils.InitRelayNetworkAccess();

        if (SteamNetworkMessagesPoller)
            return;

        Console.WriteLine("Pooler created!!!");


        // Move this to core bcs MonoBehavour uk
        /*
        SteamNetworkMessagesPoller = new SteamNetworkMessagesPoller(8);
        UnityEngine.Object.DontDestroyOnLoad(SteamNetworkMessagesPoller);
        SteamNetworkMessagesPoller.OnSteamNetworkingMessageReceived += SteamNetworkMessagesPoller_OnSteamNetworkingMessageReceived;
        SteamNetworkMessagesPoller.enabled = true;
        */
    }


    private void HandleLobbyChatMessage(CSteamID SteamID, byte[] Message)
    {
        Console.WriteLine($"LobbyMessage from {SteamID}, size: {Message.Length}");
        try
        {
            Console.WriteLine(Encoding.UTF8.GetString(Message));
        }
        catch
        {
            Console.WriteLine($"Error for message:\n{string.Join(string.Empty, Message)}");
        }
    }

    private void SteamNetworkMessagesPoller_OnSteamNetworkingMessageReceived(System.Buffers.IMemoryOwner<byte> Message, int MessageLength, SteamNetworkingMessage_t SteamNetworkingMessage)
    {
        Console.WriteLine($"SteamNetworkMessagesPoller_OnSteamNetworkingMessageReceived from {SteamNetworkingMessage.m_identityPeer.GetSteamID()}");

        try
        {

            string res = Encoding.UTF8.GetString(Message.Memory.Span.Slice(0, MessageLength));

        }
        catch
        {
            Console.WriteLine("EX WHEN PARSING");
        }


        Message.Dispose();
    }
}