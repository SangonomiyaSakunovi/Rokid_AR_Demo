using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SangoUtils.UnityWebsockets
{
    public class UnityWebSocket : MonoBehaviour
    {
        private ClientWebSocket WebsocketServer { get; set; }

        private CancellationTokenSource CancellationTokenSource_Send = new();
        private CancellationTokenSource CancellationTokenSource_OnMessage = new();

        private UnityWebsocketMessageQueue PenddingSendMessageQueue = new();
        private List<UnityWebsocketMessage> ReceivedMessageQueue = new();

        private UnityEvent<UnityWebsocketMessage> OnMessageEvent = new();

        private bool IsCanReceived { get; set; } = false;

        private static UnityWebSocket? _instance;

        public static UnityWebSocket Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType(typeof(UnityWebSocket)) as UnityWebSocket;
                    if (_instance == null)
                    {
                        GameObject gameObject = new GameObject("[" + typeof(UnityWebSocket).FullName + "]");
                        _instance = gameObject.AddComponent<UnityWebSocket>();
                        gameObject.hideFlags = HideFlags.HideInHierarchy;
                        DontDestroyOnLoad(gameObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            OnMessageEvent.AddListener(UnityWebsocketEventBus.OnMessage);
        }

        private void Update()
        {
            if (PenddingSendMessageQueue.UnityWebsocketMessages.Count > 0)
            {
                string sendMessageJson = JsonSerializer.Serialize(PenddingSendMessageQueue.UnityWebsocketMessages);
                Send(sendMessageJson);
                PenddingSendMessageQueue.UnityWebsocketMessages.Clear();
            }

            if (ReceivedMessageQueue.Count > 0)
            {
                foreach (var message in ReceivedMessageQueue)
                {
                    OnMessageEvent?.Invoke(message);
                }
                ReceivedMessageQueue.Clear();
            }
        }

        public void Connect(string serverUrl)
        {
            WebsocketServer = new ClientWebSocket();
            WebsocketServer.Options.Proxy = null;
            Task task = WebsocketServer.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            task.ContinueWith(delegate (Task task1)
            {
                if (task1.IsCompletedSuccessfully)
                {
                    Debug.Log("Log in to server.");
                    bufferStream = new MemoryStream(maxMemoryStreamSize);
                    IsCanReceived = true;
                    OnPenddingReceive();
                }
                else if (task1.IsFaulted)
                {
                    Debug.Log("Reconnecting...");
                    Connect(serverUrl);
                }
            });
        }

        public void SendRequest(string message, UnityWebsocketOperationCode OpCode)
        {
            UnityWebsocketMessageHead head = new(UnityWebsocketOperationCMD.Request, OpCode);
            UnityWebsocketMessageBody body = new(UnityTimeUtils.GetCurrentTimestampInMillis(), message);
            UnityWebsocketMessage websocketMessage = new(head, body);

            PenddingSendMessageQueue.UnityWebsocketMessages.Add(websocketMessage);
        }

        private void Send(string message)
        {
            Debug.Log($"发送的数据为///{message}///");
            Debug.Log(WebsocketServer.State);
            if (WebsocketServer.State == WebSocketState.Open)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(messageBytes);
                Task task = WebsocketServer.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationTokenSource_Send.Token);
                task.ContinueWith(delegate (Task task1)
                {
                    Debug.Log("Message has Send.");
                });
            }
        }

        private readonly int maxMemoryStreamSize = 1 * 1024 * 1024;
        private MemoryStream bufferStream;
        private void OnPenddingReceive()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(bufferStream.GetBuffer());
            Task task = WebsocketServer.ReceiveAsync(buffer, CancellationTokenSource_OnMessage.Token);
            task.ContinueWith(delegate (Task task1)
            {
                if (task1.IsCompletedSuccessfully)
                {
                    lock (bufferStream)
                    {
                        string messageStr = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                        Debug.Log(messageStr);
                        try
                        {
                            var websocketMessageQueue = JsonSerializer.Deserialize<UnityWebsocketMessageQueue>(messageStr);
                            foreach (var message in websocketMessageQueue.UnityWebsocketMessages)
                            {
                                ReceivedMessageQueue.Add(message);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                        }
                    }
                }
                else if (task1.IsFaulted)
                {
                    Debug.LogWarning("Error receiving data: " + task1.Exception);
                }

                if (bufferStream.Length > maxMemoryStreamSize)
                {
                    bufferStream.SetLength(maxMemoryStreamSize);
                }

                if (IsCanReceived)
                {
                    OnPenddingReceive();
                }
                else
                {
                    Dispose();
                }
            }
            );
        }

        public void Close()
        {
            IsCanReceived = false;
            if (WebsocketServer.State != WebSocketState.None)
            {
                Task task = WebsocketServer.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                task.ContinueWith(delegate (Task task1)
                {
                    Debug.Log("Log out from server.");
                });
            }
        }

        private void Dispose()
        {
            WebsocketServer.Dispose();
        }
    }
}
