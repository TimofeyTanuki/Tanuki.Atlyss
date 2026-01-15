using Steamworks;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tanuki.Atlyss.Network.Components;

/*
 * Steam Network Messages Pooling
 * Automatically turns off if there are no subscribers to the OnSteamNetworkingMessageReceived event.
 * Manual enable is required.
 * Manually dispose messages.
 * Memory is automatically released when exceptions occur.
 */
internal sealed class SteamNetworkMessagesPoller(int BufferSize) : MonoBehaviour
{
    private readonly IntPtr[] MessagesBuffer = new IntPtr[BufferSize];
    public int LocalChannel = 0;
    private static readonly MemoryPool<byte> MessageBuffer = MemoryPool<byte>.Shared;

    public delegate void SteamNetworkingMessageReceived(IMemoryOwner<byte> Message, int MessageLength, SteamNetworkingMessage_t SteamNetworkingMessage);
    private SteamNetworkingMessageReceived? _OnSteamNetworkingMessageReceived;
    public event SteamNetworkingMessageReceived OnSteamNetworkingMessageReceived
    {
        add
        {
            _OnSteamNetworkingMessageReceived += value;
        }
        remove
        {
            _OnSteamNetworkingMessageReceived -= value;

            if (_OnSteamNetworkingMessageReceived is not null)
                return;

            enabled = false;
        }
    }

    private void OnEnable() =>
        enabled = _OnSteamNetworkingMessageReceived is not null;

    private void Update()
    {
        int MessagesCount;
        try
        {
            MessagesCount = SteamNetworkingMessages.ReceiveMessagesOnChannel(LocalChannel, MessagesBuffer, MessagesBuffer.Length);
        }
        catch
        {
            // log
            return;
        }

        for (int Index = 0; Index < MessagesCount; Index++)
        {
            SteamNetworkingMessage_t SteamNetworkingMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(MessagesBuffer[Index]);
            IMemoryOwner<byte> MemoryOwner = MessageBuffer.Rent(SteamNetworkingMessage.m_cbSize);

            try
            {
                unsafe
                {
                    fixed (byte* Destination = MemoryOwner.Memory.Span)
                    {
                        Buffer.MemoryCopy(
                            SteamNetworkingMessage.m_pData.ToPointer(),
                            Destination,
                            MemoryOwner.Memory.Length,
                            SteamNetworkingMessage.m_cbSize);
                    }
                }
            }
            catch (Exception Exception)
            {
                Debug.LogException(Exception);
            }
            finally
            {
                SteamNetworkingMessage_t.Release(MessagesBuffer[Index]);
            }

            Console.WriteLine("D5");
            try
            {
                _OnSteamNetworkingMessageReceived?.Invoke(MemoryOwner, SteamNetworkingMessage.m_cbSize, SteamNetworkingMessage);
            }
            catch (Exception Exception)
            {
                MemoryOwner.Dispose();

                Debug.LogException(Exception);
            }
        }
    }
}