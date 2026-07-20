using Steamworks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Components;

/// <summary>
/// Component that polls incoming Steam Networking Messages from a specified channel and dispatches each received message to subscribed handlers.
/// </summary>
/// <remarks>
/// Disabled by default. The component cannot be enabled until a message buffer size and at least one message handler have been configured.
/// </remarks>
public sealed class SteamNetworkingMessagePoller() : MonoBehaviour
{
    private int messageChannel = 8192;
    private int messageBufferSize = 0;
    private IntPtr[] messageBuffer = null!;
    private Action<SteamNetworkingMessage_t>? onSteamNetworkingMessage;

    /// <summary>
    /// Gets or sets the Steam Networking Messages channel to poll.
    /// </summary>
    /// <remarks>
    /// Use a unique channel that does not overlap with channels used by other systems to avoid conflicts.
    /// </remarks>
    public int MessageChannel
    {
        set => messageChannel = value;
        get => messageChannel;
    }

    /// <summary>
    /// Gets or sets the number of messages that can be received in a single polling operation.
    /// </summary>
    public int MessageBufferSize
    {
        set
        {
            if (value < 0 || messageBufferSize == value)
                return;

            messageBufferSize = value;
            messageBuffer = new IntPtr[messageBufferSize];
        }
        get => messageBufferSize;
    }

    public event Action<SteamNetworkingMessage_t> OnSteamNetworkingMessageReceived
    {
        add => onSteamNetworkingMessage += value;
        remove
        {
            onSteamNetworkingMessage -= value;

            if (onSteamNetworkingMessage is null)
                enabled = false;
        }
    }

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Awake() => enabled = false;

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void OnEnable()
    {
        if (messageBufferSize == 0 || onSteamNetworkingMessage is null)
            enabled = false;
    }

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Update()
    {
        int count = SteamNetworkingMessages.ReceiveMessagesOnChannel(messageChannel, messageBuffer, messageBuffer.Length);

        if (count == 0)
            return;

        for (int index = 0; index < count; index++)
        {
            IntPtr messagePointer = messageBuffer[index];
            SteamNetworkingMessage_t steamNetworkingMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messagePointer);

            try
            {
                onSteamNetworkingMessage?.Invoke(steamNetworkingMessage);
            }
            finally
            {
                SteamNetworkingMessage_t.Release(messagePointer);
            }
        }
    }
}
