using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SangoUtils.UnityWebsockets
{
    [Serializable]
    public class UnityWebsocketMessageQueue
    {
        [JsonPropertyName("lst")]
        public List<UnityWebsocketMessage> UnityWebsocketMessages { get; set; } = new();
    }
    
    [Serializable]
    public class UnityWebsocketMessage
    {
        public UnityWebsocketMessage() { }
        public UnityWebsocketMessage(UnityWebsocketMessageHead head, UnityWebsocketMessageBody body)
        {
            Head = head;
            Body = body;
        }

        [JsonPropertyName("handler")]
        public UnityWebsocketMessageHead Head { get; set; } = new();
        [JsonPropertyName("message")]
        public UnityWebsocketMessageBody Body { get; set; } = new();
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; } = null;
    }

    [Serializable]
    public class UnityWebsocketMessageHead
    {
        public UnityWebsocketMessageHead() { }
        public UnityWebsocketMessageHead(UnityWebsocketOperationCMD opCMD, UnityWebsocketOperationCode opCode)
        {
            OpCMD = opCMD;
            OpCode = opCode;
        }

        [JsonPropertyName("cmd")]
        public UnityWebsocketOperationCMD OpCMD { get; set; } = UnityWebsocketOperationCMD.None;
        [JsonPropertyName("op")]
        public UnityWebsocketOperationCode OpCode { get; set; } = UnityWebsocketOperationCode.None;
    }

    [Serializable]
    public class UnityWebsocketMessageBody
    {
        public UnityWebsocketMessageBody() { }
        public UnityWebsocketMessageBody(long timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }

        [JsonPropertyName("ts")]
        public long Timestamp { get; set; } = 0;
        [JsonPropertyName("msg")]
        public string Message { get; set; } = string.Empty;
    }

    [Serializable]
    public enum UnityWebsocketOperationCMD
    {
        None,
        Request,
        Response,
        Event
    }

    [Serializable]
    public enum UnityWebsocketOperationCode
    {
        None,
        Ping,
        Login,
        Regist,
        Pose,
        OpKey
    }

    [Serializable]
    public enum UnityWebsocketOperationRes
    {
        None,
        Success,
        Failed,
        Pendding
    }
}
