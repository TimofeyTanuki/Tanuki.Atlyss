using Steamworks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Components;

public sealed class SteamNetworkMessagePoller() : MonoBehaviour
{
    private IntPtr[] messageBuffer = null!;
    private int messageBufferSize = 0;
    public int LocalChannel;
    public Action? onSteamNetworkingMessages;
    public Action<SteamNetworkingMessage_t>? steamNetworkingMessageHandler;

    public event Action OnSteamNetworkingMessages
    {
        add => onSteamNetworkingMessages += value;
        remove
        {
            onSteamNetworkingMessages -= value;

            if (onSteamNetworkingMessages is null)
                enabled = false;
        }
    }

    public event Action<SteamNetworkingMessage_t> SteamNetworkingMessageHandler
    {
        add => steamNetworkingMessageHandler += value;
        remove
        {
            steamNetworkingMessageHandler -= value;

            if (steamNetworkingMessageHandler is null)
                enabled = false;
        }
    }

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

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Awake() =>
        enabled = false;

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void OnEnable()
    {
        if (messageBufferSize == 0 ||
            onSteamNetworkingMessages is null ||
            steamNetworkingMessageHandler is null)
            enabled = false;
    }

    [SuppressMessage("CodeQuality", "IDE0051")]
    private void Update()
    {
        int count = SteamNetworkingMessages.ReceiveMessagesOnChannel(LocalChannel, messageBuffer, messageBuffer.Length);

        if (count == 0)
            return;

        Console.WriteLine($"Count: {count}");

        onSteamNetworkingMessages!();

        for (int index = 0; index < count; index++)
        {
            IntPtr messagePointer = messageBuffer[index];
            SteamNetworkingMessage_t steamNetworkingMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messagePointer);
            steamNetworkingMessageHandler!(steamNetworkingMessage);
            SteamNetworkingMessage_t.Release(messagePointer);
        }
    }
}
