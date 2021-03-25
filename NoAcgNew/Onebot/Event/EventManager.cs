using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NoAcgNew.EventArgs.OneBotEventArgs.MetaEventArgs;
using NoAcgNew.Interfaces;
using NoAcgNew.OnebotModel.OnebotEvent.MessageEvent;
using NoAcgNew.OnebotModel.OnebotEvent.MetaEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Newtonsoft.Json;
using NoAcgNew.Onebot.Models;
using NoAcgNew.Onebot.Models.ApiParams;
using WebApiClientCore;

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
        /// <typeparam name="TResult">返回</typeparam>
        /// <param name="eventArgs">事件参数</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        public delegate ValueTask<TResult> EventCallBackHandler<in TEventArgs, TResult>(TEventArgs eventArgs,
            IOneBotApi oneBotApi)
            where TEventArgs : System.EventArgs where TResult : BaseEventReturn;

        public delegate ValueTask<int> EventCallBackHandler<in TEventArgs>(TEventArgs eventArgs, IOneBotApi oneBotApi)
            where TEventArgs : System.EventArgs;

        #endregion

        #region 事件回调

        /// <summary>
        /// 心跳事件
        /// </summary>
        public EventCallBackHandler<HeartBeatEventArgs> OnHeartBeatEvent;

        /// <summary>
        /// 生命周期事件
        /// </summary>
        public EventCallBackHandler<LifeCycleEventArgs> OnLifeCycleEvent;

        /// <summary>
        /// 私聊事件
        /// </summary>
        public EventCallBackHandler<PrivateMsgEventArgs, PrivateMsgReturn> OnPrivateMessage;

        /// <summary>
        /// 群聊事件
        /// </summary>
        public EventCallBackHandler<GroupMsgEventArgs, GroupMsgReturn> OnGroupMessage;
        
        #endregion

        #region 事件分发

        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="messageJson">消息json对象</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        /// <param name="rawMsg"></param>
        internal async ValueTask<object> Adapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var type = GetBaseEventType(messageJson);
            try
            {
                switch (type)
                {
                    //元事件类型
                    case "meta_event":
                        await MetaAdapter(messageJson, oneBotApi, rawMsg);
                        break;
                    case "message":
                        return await MessageAdapter(messageJson, oneBotApi, rawMsg);
                    default:
                        _logger.LogWarning("[Event]接收到未知事件[{Msg}]", rawMsg);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Adapter]事件解析出现未知错误：{Msg}", rawMsg);
            }

            return null;
        }

        /// <summary>
        /// 元事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        /// <param name="rawMsg"></param>
        private async ValueTask MetaAdapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var type = GetMetaEventType(messageJson);
            switch (type)
            {
                //心跳包
                case "heartbeat":
                {
                    var heartBeat = messageJson.ToObject<HeartBeatEventArgs>();
                    if (heartBeat != null && OnHeartBeatEvent != null)
                        await InvokeEvent(OnHeartBeatEvent, heartBeat, oneBotApi, rawMsg);
                    break;
                }
                //生命周期
                case "lifecycle":
                {
                    var lifeCycle = messageJson.ToObject<LifeCycleEventArgs>();
                    if (lifeCycle != null && OnLifeCycleEvent != null)
                        await InvokeEvent(OnLifeCycleEvent, lifeCycle, oneBotApi, rawMsg);
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
        /// <param name="rawMsg"></param>
        private async ValueTask<object> MessageAdapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var type = GetMessageType(messageJson);
            switch (type)
            {
                //私聊事件
                case "private":
                {
                    var privateMsg = messageJson.ToObject<PrivateMsgEventArgs>();
                    if (privateMsg != null && OnPrivateMessage != null)
                        return await InvokeEvent(OnPrivateMessage, privateMsg, oneBotApi, rawMsg);
                    break;
                }
                //群聊事件
                case "group":
                {
                    var groupMsg = messageJson.ToObject<GroupMsgEventArgs>();
                    if (groupMsg != null && OnGroupMessage != null)
                        return await InvokeEvent(OnGroupMessage, groupMsg, oneBotApi, rawMsg);
                    break;
                }
                default:
                    _logger.LogWarning("[Message Event]接收到未知事件[{MessageEventType}]", type);
                    break;
            }

            return null;
        }

        private async ValueTask InvokeEvent<T>(EventCallBackHandler<T> handler, T args, IOneBotApi api, string rawMsg)
            where T : System.EventArgs
        {
            foreach (var @delegate in handler.GetInvocationList())
            {
                var func = (EventCallBackHandler<T>) @delegate;
                var code = 0;
                try
                {
                    code = await func(args, api);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "消息处理出现未知错误：{Msg}", rawMsg);
                }

                if (code == 1) break;
            }
        }

        private async ValueTask<TResult> InvokeEvent<T, TResult>(EventCallBackHandler<T, TResult> handler, T args,
            IOneBotApi api, string rawMsg)
            where T : System.EventArgs where TResult : BaseEventReturn
        {
            TResult replay = null;
            foreach (var @delegate in handler.GetInvocationList())
            {
                var func = (EventCallBackHandler<T, TResult>) @delegate;
                var code = 0;
                try
                {
                    var result = await func(args, api);
                    if (result == null) continue;
                    replay = result;
                    code = result.Code;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "消息处理出现未知错误：{Msg}", rawMsg);
                }

                if (code == 1) break;
            }

            return replay;
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