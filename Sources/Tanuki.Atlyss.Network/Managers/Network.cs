using Steamworks;
using System;
using System.Text;
using Tanuki.Atlyss.Network.Components;

namespace Tanuki.Atlyss.Network.Managers;

internal class Network
{
    private const ushort LobbyChatMsg_BufferSize = 4096;

    public static Network Instance = null!;
    private static Callback<LobbyChatMsg_t> LobbyChatMessage = null!;
    private static Callback<SteamNetworkingMessagesSessionRequest_t> SteamNetworkingMessagesSessionRequest = null!;
    private bool IsSteamInitialized = false;
    private SteamNetworkMessagesPoller? SteamNetworkMessagesPoller;

    private Network()
    {
        LobbyChatMessage = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
        SteamNetworkingMessagesSessionRequest = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(OnSteamNetworkingMessagesSessionRequest);
        Callback<SteamRelayNetworkStatus_t>.Create(OnSteamRelayNetworkStatus);
    }

    public static void Initialize() => Instance ??= new();

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

    private void OnLobbyChatMessage(LobbyChatMsg_t LobbyChatMsg)
    {
        Console.WriteLine("OnLobbyChatMessage");

        byte[] Buffer = new byte[LobbyChatMsg_BufferSize];
        int MessageLength = SteamMatchmaking.GetLobbyChatEntry(new CSteamID(LobbyChatMsg.m_ulSteamIDLobby), (int)LobbyChatMsg.m_iChatID, out CSteamID SteamID, Buffer, Buffer.Length, out EChatEntryType ChatEntryType);

        byte[] messageBuffer = new byte[MessageLength];
        Array.Copy(Buffer, messageBuffer, MessageLength);

        HandleLobbyChatMessage(SteamID, messageBuffer);
    }

    private void OnSteamNetworkingMessagesSessionRequest(SteamNetworkingMessagesSessionRequest_t SteamNetworkingMessagesSessionRequest)
    {
        Console.WriteLine("OnSteamNetworkingMessagesSessionRequest");
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
}