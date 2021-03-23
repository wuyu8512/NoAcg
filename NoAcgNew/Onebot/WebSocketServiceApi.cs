using System.Net.WebSockets;
using NoAcgNew.Interfaces;

namespace NoAcgNew.Onebot
{
    public class WebSocketServiceApi : IOneBotApi
    {
        private readonly WebSocket _socket;
        
        public WebSocketServiceApi(WebSocket socket)
        {
            _socket = socket;
        }
    }
}