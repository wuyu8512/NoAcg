using System.ComponentModel;

namespace NoAcgNew.Onebot.Models
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