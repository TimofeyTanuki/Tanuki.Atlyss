using Steamworks;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Components;

/*
 * Steam Network Messages Pooling
 * Manual enable is required.
 */
internal sealed class SteamNetworkMessagesPoller() : MonoBehaviour
{
    private static MemoryPool<byte> memoryPool = null!;
    private IntPtr[] messageBuffer = null!;
    private Action<ReadOnlyMemory<byte>, int, SteamNetworkingMessage_t> handler = null!;
    public int localChannel;

    public Action<ReadOnlyMemory<byte>, int, SteamNetworkingMessage_t>? Handler
    {
        set
        {
            if (value is null)
            {
                enabled = false;
                return;
            }

            handler = value;
        }
        get => handler;
    }

    public int MessageBufferSize
    {
        set => messageBuffer = new IntPtr[value];
        get => messageBuffer is null ? 0 : messageBuffer.Length;
    }

    private void OnEnable()
    {
        if (messageBuffer is null || handler is null)
        {
            enabled = false;
            return;
        }

        memoryPool ??= MemoryPool<byte>.Shared;
    }

    private void Update()
    {
        int count = SteamNetworkingMessages.ReceiveMessagesOnChannel(localChannel, messageBuffer, messageBuffer.Length);

        for (int index = 0; index < count; index++)
        {
            IntPtr messagePointer = messageBuffer[index];

            SteamNetworkingMessage_t message = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messagePointer);

            SteamNetworkingMessage_t.Release(messagePointer);

            IMemoryOwner<byte> buffer = memoryPool.Rent(message.m_cbSize);

            try
            {
                unsafe
                {
                    fixed (byte* destination = &MemoryMarshal.GetReference(buffer.Memory.Span))
                    {
                        Buffer.MemoryCopy(
                            message.m_pData.ToPointer(),
                            destination,
                            buffer.Memory.Length,
                            message.m_cbSize);
                    }
                }

                handler.Invoke(buffer.Memory[..message.m_cbSize], message.m_cbSize, message);
            }
            finally
            {
                buffer.Dispose();
            }
        }
    }
}
