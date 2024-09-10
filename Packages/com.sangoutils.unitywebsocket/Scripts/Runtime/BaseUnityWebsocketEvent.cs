using System;
using System.Text.Json;

namespace SangoUtils.UnityWebsockets
{
    public abstract class BaseUnityWebsocketEvent
    {
        internal protected abstract UnityWebsocketOperationCode OpCode { get; }

        internal protected abstract void OnEvent(UnityWebsocketMessageBody messageBody);

        public BaseUnityWebsocketEvent()
        {
            UnityWebsocketEventBus.AddEvent(this);
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
