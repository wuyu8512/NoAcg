using System;
using Microsoft.Extensions.Configuration;

namespace Wuyu.OneBot
{
    public class OneBotOptions
    {
        public bool EnableWebSocketService { get; set; }
        public bool EnableWebSocketClient { get; set; }
        public string WebSocketServiceUrl { get; set; } = "/Universal";
        public string WebSocketClientUrl { get; set; }
        public bool EnableHttpPost { get; set; }
        public IConfigurationSection HttpApi { get; set; }
    }
}