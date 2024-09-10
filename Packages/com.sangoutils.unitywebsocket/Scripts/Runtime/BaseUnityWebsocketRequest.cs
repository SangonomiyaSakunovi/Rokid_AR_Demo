using System;
using System.Text.Json;

namespace SangoUtils.UnityWebsockets
{
    public abstract class BaseUnityWebsocketRequest
    {
        internal protected abstract UnityWebsocketOperationCode OpCode { get; }

        internal protected abstract void OnResponse(UnityWebsocketMessageBody messageBody);

        public BaseUnityWebsocketRequest()
        {
            UnityWebsocketEventBus.AddRequest(this);
        }

        protected void SendRequest(string message)
        {
            UnityWebSocket.Instance.SendRequest(message, OpCode);
        }

        protected string ToJson(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        protected T? FromJson<T>(string str) where T : class
        {
            T? t;
            try
            {
                t = JsonSerializer.Deserialize<T>(str);
            }
            catch (Exception)
            {
                throw;
            }
            return t;
        }
    }
}
