using BepInEx.Logging;
using Steamworks;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Tanuki.Atlyss.Network.Components;
using Tanuki.Atlyss.Network.Providers;

namespace Tanuki.Atlyss.Network.Managers;

public sealed class Network
{
    private readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;

    private readonly ManualLogSource manualLogSource;
    private readonly Steam steamProvider;
    private readonly Providers.SteamLobby steamLobbyProvider;
    private readonly SteamNetworkMessagePoller steamNetworkMessagePoller;
    private readonly Registers.Packets packetRegistry;
    private readonly Services.RateLimiter rateLimiter;
    private readonly Routers.Packets packetRouter;

    private int steamLocalChannel;
    public bool PreventLobbyOwnerRateLimiting;

    public Services.RateLimiter RateLimiter => rateLimiter;
    public SteamNetworkMessagePoller SteamNetworkMessagesPoller => steamNetworkMessagePoller;
    public int SteamLocalChannel
    {
        get => steamLocalChannel;
        set
        {
            steamLocalChannel = value;
            packetRouter.steamLocalChannel = SteamNetworkMessagesPoller.LocalChannel = value;
        }
    }

    internal Network(ManualLogSource manualLogSource,
        Steam steamProvider,
        Providers.SteamLobby steamLobbyProvider,
        SteamNetworkMessagePoller steamNetworkMessagePoller,
        Registers.Packets packetRegistry,
        Services.RateLimiter rateLimiter,
        Routers.Packets packetRouter)
    {
        this.manualLogSource = manualLogSource;
        this.steamProvider = steamProvider;
        this.steamLobbyProvider = steamLobbyProvider;
        this.steamNetworkMessagePoller = steamNetworkMessagePoller;
        this.packetRegistry = packetRegistry;
        this.rateLimiter = rateLimiter;
        this.packetRouter = packetRouter;

        steamNetworkMessagePoller.onSteamNetworkingMessages = OnSteamNetworkingMessages;
        steamNetworkMessagePoller.steamNetworkingMessageHandler = SteamNetworkMessageHandler;

        steamProvider.OnLobbyChatUpdate += OnLobbyChatUpdate;
        steamProvider.OnSteamRelayNetworkStatus += OnSteamRelayNetworkStatus;
        steamProvider.OnSteamNetworkingMessagesSessionRequest += OnSteamNetworkingMessagesSessionRequest;
        steamProvider.OnLobbyChatMsg += OnLobbyChatMsg;

        steamLobbyProvider.OnLobbyChanged += OnLobbyChanged;
    }

    private void OnLobbyChanged(CSteamID lobbyId)
    {
        if (lobbyId.Equals(CSteamID.Nil))
            rateLimiter.Reset();
    }

    private bool CheckBandwidthOverflow(CSteamID sender, uint usage)
    {
        if (sender.IsLobby() ||
            PreventLobbyOwnerRateLimiting && sender.Equals(steamLobbyProvider.OwnerSteamId))
        {
            Console.WriteLine($"PREVENTED FOR {sender.m_SteamID}");
            return false;
        }

        return rateLimiter.CheckBandwidthOverflow(sender, usage);
    }

    private void OnLobbyChatMsg(LobbyChatMsg_t lobbyChatMsg)
    {
        rateLimiter.Tick();
        byte[] buffer = arrayPool.Rent(Tanuki.PACKET_MAX_SIZE);

        int size = SteamMatchmaking.GetLobbyChatEntry(steamLobbyProvider.OwnerSteamId, (int)lobbyChatMsg.m_iChatID, out CSteamID sender, buffer, Tanuki.PACKET_MAX_SIZE, out EChatEntryType _);

        if (CheckBandwidthOverflow(sender, (uint)size))
            return;

        packetRouter.ReceivePacket(sender, buffer);
        arrayPool.Return(buffer);
    }

    private void OnSteamNetworkingMessagesSessionRequest(SteamNetworkingMessagesSessionRequest_t steamNetworkingMessagesSessionRequest)
    {
        rateLimiter.Tick();
        manualLogSource.LogInfo($"OnSteamNetworkingMessagesSessionRequest {steamNetworkingMessagesSessionRequest.m_identityRemote.GetSteamID64()}");

        if (CheckBandwidthOverflow(steamNetworkingMessagesSessionRequest.m_identityRemote.GetSteamID(), 0))
            return;

        SteamNetworkingMessages.AcceptSessionWithUser(ref steamNetworkingMessagesSessionRequest.m_identityRemote);
    }

    private void OnSteamRelayNetworkStatus(SteamRelayNetworkStatus_t steamRelayNetworkStatus)
    {
        if (steamRelayNetworkStatus.m_eAvail != ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current)
            return;

        steamProvider.OnSteamRelayNetworkStatus -= OnSteamRelayNetworkStatus;
        SteamNetworkingUtils.InitRelayNetworkAccess();
        steamNetworkMessagePoller.enabled = true;
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t lobbyChatUpdate)
    {
        EChatMemberStateChange state = (EChatMemberStateChange)lobbyChatUpdate.m_rgfChatMemberStateChange;

        if ((state & EChatMemberStateChange.k_EChatMemberStateChangeLeft) != 0)
            rateLimiter.Reset(lobbyChatUpdate.m_ulSteamIDUserChanged);
    }

    private void OnSteamNetworkingMessages() =>
        rateLimiter.Tick();

    private void SteamNetworkMessageHandler(SteamNetworkingMessage_t steamNetworkingMessage)
    {
        CSteamID sender = steamNetworkingMessage.m_identityPeer.GetSteamID();

        if (rateLimiter.CheckBandwidthOverflow(sender, (uint)steamNetworkingMessage.m_cbSize))
        {
            SteamNetworkingMessages.CloseSessionWithUser(ref steamNetworkingMessage.m_identityPeer);
            return;
        }

        byte[] buffer = arrayPool.Rent(steamNetworkingMessage.m_cbSize);

        try
        {
            unsafe
            {
                fixed (byte* destination = &MemoryMarshal.GetReference(buffer))
                {
                    Buffer.MemoryCopy(
                        steamNetworkingMessage.m_pData.ToPointer(),
                        destination,
                        buffer.Length,
                        steamNetworkingMessage.m_cbSize);
                }
            }
        }
        catch
        {
            arrayPool.Return(buffer);
            return;
        }

        packetRouter.ReceivePacket(sender, buffer);
        arrayPool.Return(buffer);
    }
}
