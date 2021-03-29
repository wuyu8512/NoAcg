using System;

namespace Wuyu.OneBot
{
    public class OneBotOptions
    {
        public bool EnableWebSocketService { get; set; }
        public string WebSocketUrl { get; set; }
        public bool EnableHttpPost { get; set; }
        public Uri HttpApiHost { get; set; }
    }
}