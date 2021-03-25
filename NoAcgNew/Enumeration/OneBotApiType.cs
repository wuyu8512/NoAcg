using System.ComponentModel;

namespace Enumeration
{
    public enum OneBotApiType
    {
        /// <summary>
        /// Http
        /// </summary>
        [Description("http")] Http,

        /// <summary>
        /// WebSocket
        /// </summary>
        [Description("websocket")] WebSocket,
    }
}