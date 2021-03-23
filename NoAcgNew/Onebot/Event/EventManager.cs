using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NoAcgNew.EventArgs.OneBotEventArgs.MetaEventArgs;
using NoAcgNew.Interfaces;
using Sora.OnebotModel.OnebotEvent.MessageEvent;
using Sora.OnebotModel.OnebotEvent.MetaEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Event
{
    public class EventManager
    {
        private readonly ILogger<EventManager> _logger;

        public EventManager(ILogger<EventManager> logger)
        {
            _logger = logger;
        }

        #region 事件委托

        /// <summary>
        /// Onebot事件回调
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数</typeparam>
        /// <param name="eventArgs">事件参数</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        public delegate ValueTask EventAsyncCallBackHandler<in TEventArgs>(TEventArgs eventArgs, IOneBotApi oneBotApi)
            where TEventArgs : System.EventArgs;

        #endregion

        #region 事件回调

        /// <summary>
        /// 心跳事件
        /// </summary>
        public event EventAsyncCallBackHandler<HeartBeatEventArgs> OnHeartBeatEvent;

        /// <summary>
        /// 生命周期事件
        /// </summary>
        public event EventAsyncCallBackHandler<LifeCycleEventArgs> OnLifeCycleEvent;

        /// <summary>
        /// 私聊事件
        /// </summary>
        public event EventAsyncCallBackHandler<PrivateMsgEventArgs> OnPrivateMessage;

        #endregion

        #region 事件分发

        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="messageJson">消息json对象</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        internal async ValueTask Adapter(JObject messageJson, IOneBotApi oneBotApi)
        {
            var type = GetBaseEventType(messageJson);
            try
            {
                switch (type)
                {
                    //元事件类型
                    case "meta_event":
                        await MetaAdapter(messageJson, oneBotApi);
                        break;
                    case "message":
                        await MessageAdapter(messageJson, oneBotApi);
                        break;
                    default:
                        _logger.LogWarning("[Event]接收到未知事件[{EventType}]", type);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Adapter]事件解析出现未知错误：{Data}", messageJson.ToString(Formatting.None));
            }
        }

        /// <summary>
        /// 元事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        private async ValueTask MetaAdapter(JObject messageJson, IOneBotApi oneBotApi)
        {
            var type = GetMetaEventType(messageJson);
            switch (type)
            {
                //心跳包
                case "heartbeat":
                {
                    var heartBeat = messageJson.ToObject<HeartBeatEventArgs>();
                    if (heartBeat != null && OnHeartBeatEvent != null)
                        await OnHeartBeatEvent(heartBeat, oneBotApi);
                    break;
                }
                //生命周期
                case "lifecycle":
                {
                    var lifeCycle = messageJson.ToObject<LifeCycleEventArgs>();
                    if (lifeCycle != null && OnLifeCycleEvent != null) await OnLifeCycleEvent(lifeCycle, oneBotApi);
                    break;
                }
                default:
                    _logger.LogWarning("[Meta Event]接收到未知事件[{MetaEventType}]", type);
                    break;
            }
        }

        /// <summary>
        /// 消息事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        private async ValueTask MessageAdapter(JObject messageJson, IOneBotApi oneBotApi)
        {
            var type = GetMessageType(messageJson);
            switch (type)
            {
                //私聊事件
                case "private":
                {
                    var privateMsg = messageJson.ToObject<PrivateMsgEventArgs>();
                    if (privateMsg != null && OnPrivateMessage != null) await OnPrivateMessage(privateMsg, oneBotApi);
                    break;
                }
                //群聊事件
                case "group":
                {
                    break;
                }
                default:
                    _logger.LogWarning("[Message Event]接收到未知事件[{MetaEventType}]", type);
                    break;
            }
        }

        #endregion

        #region 事件类型获取

        /// <summary>
        /// 获取上报事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetBaseEventType(JObject messageJson) =>
            !messageJson.TryGetValue("post_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取元事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMetaEventType(JObject messageJson) =>
            !messageJson.TryGetValue("meta_event_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取消息事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMessageType(JObject messageJson) =>
            !messageJson.TryGetValue("message_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取请求事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetRequestType(JObject messageJson) =>
            !messageJson.TryGetValue("request_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetNoticeType(JObject messageJson) =>
            !messageJson.TryGetValue("notice_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件子类型
        /// </summary>
        /// <param name="messageJson"></param>
        private static string GetNotifyType(JObject messageJson) =>
            !messageJson.TryGetValue("sub_type", out JToken typeJson) ? string.Empty : typeJson.ToString();

        #endregion
    }
}