using System.Net.WebSockets;
using NoAcgNew.Interfaces;
using NoAcgNew.Onebot.Models;

namespace NoAcgNew.Onebot
{
    public class WebSocketServiceApi : IOneBotApi
    {
        private readonly WebSocket _socket;
        
        public WebSocketServiceApi(WebSocket socket)
        {
            _socket = socket;
        }

        public OneBotApiType GetApiType()
        {
            return OneBotApiType.WebSocket;
        }
    }
}