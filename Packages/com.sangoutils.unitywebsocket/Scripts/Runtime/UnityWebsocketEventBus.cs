using System.Collections.Concurrent;

namespace SangoUtils.UnityWebsockets
{
    internal static class UnityWebsocketEventBus
    {
        private static readonly ConcurrentDictionary<UnityWebsocketOperationCode, BaseUnityWebsocketRequest> _requestDic = new();
        private static readonly ConcurrentDictionary<UnityWebsocketOperationCode, BaseUnityWebsocketEvent> _eventDic = new();

        public static bool AddRequest(BaseUnityWebsocketRequest req)
        {
            if (!_requestDic.ContainsKey(req.OpCode))
            {
                return _requestDic.TryAdd(req.OpCode, req);
            }
            return true;
        }
        public static bool AddEvent(BaseUnityWebsocketEvent evt)
        {
            if (!_eventDic.ContainsKey(evt.OpCode))
            {
                return _eventDic.TryAdd(evt.OpCode, evt);
            }
            return true;
        }

        public static void OnMessage(UnityWebsocketMessage message)
        {
            switch (message.Head.OpCMD)
            {
                case UnityWebsocketOperationCMD.Response:
                    if (_requestDic.TryGetValue(message.Head.OpCode, out var req))
                    {
                        req.OnResponse(message.Body);
                    }
                    break;
                case UnityWebsocketOperationCMD.Event:
                    if (_eventDic.TryGetValue(message.Head.OpCode, out var evt))
                    {
                        evt.OnEvent(message.Body);
                    }
                    break;
            }
        }
    }
}
