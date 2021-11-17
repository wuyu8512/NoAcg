using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.EventArgs.MetaEvent;
using Wuyu.OneBot.Models.EventArgs.NoticeEvent;
using Wuyu.OneBot.Models.EventArgs.RequestEvent;
using Wuyu.OneBot.Models.QuickOperation;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;
using Wuyu.OneBot.Models.QuickOperation.RequestQuickOperation;

namespace Wuyu.OneBot
{
    public class EventManager
    {
        private readonly ILogger<EventManager> _logger;
        private readonly List<(Type type, EventTypeAttribute customAttributes)> _eventTypes = new();
        private readonly EventInfo[] _eventInfos;
        private readonly MethodInfo _invokeEventResultInfo;
        private readonly MethodInfo _invokeEventInfo;

        public EventManager(ILogger<EventManager> logger)
        {
            _logger = logger;

            var assembly = Assembly.GetExecutingAssembly();
            assembly.GetTypes().ToList().ForEach(type =>
            {
                if (type.IsSubclassOf(typeof(BaseEventArgs)))
                {
                    var customAttributes = type.GetCustomAttributes(typeof(EventTypeAttribute), false).FirstOrDefault();
                    if (customAttributes != null)
                    {
                        _eventTypes.Add((type, customAttributes as EventTypeAttribute));
                    }
                }
            });

            var eventManager = typeof(EventManager);
            _eventInfos = eventManager.GetEvents();
            _invokeEventResultInfo = eventManager.GetMethod("InvokeEventResult", BindingFlags.Instance | BindingFlags.NonPublic);
            _invokeEventInfo = eventManager.GetMethod("InvokeEvent", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        #region 事件委托

        /// <summary>
        /// Onebot事件回调，带参数和返回值版本
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数</typeparam>
        /// <typeparam name="TResult">返回</typeparam>
        /// <param name="eventArgs">事件参数</param>
        /// <param name="oneBotApi">对应的OneBot Api接口</param>
        public delegate ValueTask<TResult> EventCallBackHandler<in TEventArgs, TResult>(TEventArgs eventArgs,
            IOneBotApi oneBotApi)
            where TEventArgs : EventArgs where TResult : BaseQuickOperation;

        /// <summary>
        /// Onebot事件回调，带参数版本
        /// </summary>
        /// <typeparam name="TEventArgs">事件参数</typeparam>
        /// <param name="eventArgs">事件参数</param>
        /// <param name="oneBotApi">对应的OneBot Api接口</param>
        /// <returns></returns>
        public delegate ValueTask<int> EventCallBackHandler<in TEventArgs>(TEventArgs eventArgs, IOneBotApi oneBotApi);

        /// <summary>
        /// Onebot事件回调
        /// </summary>
        /// <param name="oneBotApi">对应的OneBot Api接口</param>
        /// <returns></returns>
        public delegate ValueTask EventCallBackHandler(IOneBotApi oneBotApi);

        #endregion

        #region 事件回调

        /// <summary>
        /// 连接事件，对于反向WebSocket，将在每次客户端连接时触发，对于Http Post，将在程序启动时触发
        /// </summary>
        public event EventCallBackHandler OnConnection;

        /// <summary>
        /// 心跳事件
        /// </summary>
        public event EventCallBackHandler<HeartBeatEventArgs> OnHeartBeatEvent;

        /// <summary>
        /// 生命周期事件
        /// </summary>
        public event EventCallBackHandler<LifeCycleEventArgs> OnLifeCycleEvent;

        /// <summary>
        /// 私聊事件
        /// </summary>
        public event EventCallBackHandler<PrivateMsgEventArgs, PrivateMsgQuickOperation> OnPrivateMessage;

        /// <summary>
        /// 群聊事件
        /// </summary>
        public event EventCallBackHandler<GroupMsgEventArgs, GroupMsgQuickOperation> OnGroupMessage;

        /// <summary>
        /// 群文件上传事件
        /// </summary>
        public event EventCallBackHandler<FileUploadEventArgs> OnFileUpload;

        /// <summary>
        /// 群管理员变动事件
        /// </summary>
        public event EventCallBackHandler<AdminChangeEventArgs> OnGroupAdminChange;

        /// <summary>
        /// 群成员数量变动事件
        /// </summary>
        public event EventCallBackHandler<GroupMemberChangeEventArgs> OnGroupMemberChange;

        /// <summary>
        /// 群禁言事件
        /// </summary>
        public event EventCallBackHandler<GroupBanEventArgs> OnGroupBan;

        /// <summary>
        /// 好友添加事件
        /// </summary>
        public event EventCallBackHandler<FriendAddEventArgs> OnFriendAdd;

        /// <summary>
        /// 群消息撤回事件
        /// </summary>
        public event EventCallBackHandler<GroupRecallEventArgs> OnGroupRecall;

        /// <summary>
        /// 好友消息撤回事件
        /// </summary>
        public event EventCallBackHandler<FriendRecallEventArgs> OnFriendRecall;

        /// <summary>
        /// 好友戳一戳事件
        /// </summary>
        public event EventCallBackHandler<PokeEventArgs> OnFriendPokeEvent;

        /// <summary>
        /// 群内戳一戳事件
        /// </summary>
        public event EventCallBackHandler<PokeEventArgs> OnGroupPokeEvent;

        /// <summary>
        /// 群红包运气王提示事件
        /// </summary>
        public event EventCallBackHandler<LuckyKingEventArgs> OnLuckyKingEvent;

        /// <summary>
        /// 群成员荣誉变更事件
        /// </summary>
        public event EventCallBackHandler<HonorEventArgs> OnHonorEvent;

        /// <summary>
        /// 群成员名片更新事件
        /// </summary>
        public event EventCallBackHandler<GroupCardUpdateEventArgs> OnGroupCardUpdate;

        /// <summary>
        /// 接收到离线文件事件
        /// </summary>
        public event EventCallBackHandler<OfflineFileEventArgs> OnOfflineFileEvent;

        /// <summary>
        /// 其他客户端在线状态变更事件
        /// </summary>
        public event EventCallBackHandler<ClientStatusChangeEventArgs> OnClientStatusChange;

        /// <summary>
        /// 精华消息事件
        /// </summary>
        public event EventCallBackHandler<EssenceChangeEventArgs> OnEssenceChange;

        /// <summary>
        /// 加好友请求事件
        /// </summary>
        public event EventCallBackHandler<FriendRequestEventArgs, FriendRequestQuickOperation> OnFriendRequest;

        /// <summary>
        /// 加群请求／邀请事件
        /// </summary>
        public event EventCallBackHandler<GroupRequestEventArgs, GroupRequestQuickOperation> OnGroupRequest;

        #endregion

        #region 事件分发

        public async void Connection(IOneBotApi oneBotApi)
        {
            try
            {
                if (OnConnection != null) await OnConnection(oneBotApi);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "出现了未知错误");
            }
        }

        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="messageJson">消息json对象</param>
        /// <param name="oneBotApi">客户端链接接口</param>
        /// <param name="rawMsg">原始文本</param>
        public async ValueTask<object> Adapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var postType = messageJson.TryGetValue("post_type", out var postTypeJson) ? postTypeJson.ToString() : null;
            if (postType == null)
            {
                _logger.LogWarning("[Event]接收到未知事件: {Msg}", rawMsg);
                return null;
            }
            var eventType = messageJson.TryGetValue($"{postType}_type", out var typeJson) ? typeJson.ToString() : null;
            var subType = messageJson.TryGetValue($"sub_type", out var subTypeJson) ? subTypeJson.ToString() : null;

            var eventClass = _eventTypes.Where((type) =>
            {
                return type.customAttributes is EventTypeAttribute eventAttributes && eventAttributes.PostType == postType &&
                                eventAttributes.Type.Contains(eventType) && (eventAttributes.SubType == subType || eventAttributes.SubType == "allType");
            }).Select(item => item.type).FirstOrDefault();
            if (eventClass == null)
            {
                _logger.LogWarning("[Event]接收到未知事件: {Msg}", rawMsg);
                return null;
            }
            var args = messageJson.ToObject(eventClass);
            var eventManager = typeof(EventManager);
            var eventInfo = _eventInfos.FirstOrDefault(item => item.EventHandlerType.GenericTypeArguments.Contains(eventClass));
            if (args == null || eventInfo == default)
            {
                _logger.LogWarning("[Event]接收到未知事件: {Msg}", rawMsg);
                return null;
            }

            try
            {
                if (eventInfo.EventHandlerType.GenericTypeArguments.Length == 1)
                {
                    // 带参数，不带返回值
                    var delegateInstance = eventManager.GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
                    if (delegateInstance != null)
                    {
                        // 特殊处理某些Event
                        if (args is PokeEventArgs pokeEvent)
                        {
                            if (pokeEvent.GroupId == default)
                            {
                                if (OnGroupPokeEvent != null) await InvokeEvent(OnGroupPokeEvent, pokeEvent, oneBotApi, rawMsg);
                            }
                            else if (OnFriendPokeEvent != null) await InvokeEvent(OnFriendPokeEvent, pokeEvent, oneBotApi, rawMsg);
                        }
                        else
                        {
                            MethodInfo bound = _invokeEventInfo.MakeGenericMethod(eventClass);
                            dynamic result = bound.Invoke(this, new object[] { delegateInstance, args, oneBotApi, rawMsg });
                            await result;
                        }
                    }
                }
                else if (eventInfo.EventHandlerType.GenericTypeArguments.Length == 2)
                {
                    // 带参数和返回值
                    var delegateInstance = eventManager.GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);

                    if (delegateInstance != null)
                    {
                        MethodInfo bound = _invokeEventResultInfo.MakeGenericMethod(eventClass, eventInfo.EventHandlerType.GenericTypeArguments[1]);
                        dynamic result = bound.Invoke(this, new object[] { delegateInstance, args, oneBotApi, rawMsg });
                        return await result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Adapter]事件处理出现未知错误：{Msg}", rawMsg);
            }

            return null;
        }

        #endregion

        #region 事件处理

        private async ValueTask InvokeEvent<T>(EventCallBackHandler<T> handler, T args, IOneBotApi api, string rawMsg)
            where T : EventArgs
        {
            foreach (var @delegate in handler.GetInvocationList())
            {
                var func = (EventCallBackHandler<T>)@delegate;
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

        private async ValueTask<TResult> InvokeEventResult<T, TResult>(EventCallBackHandler<T, TResult> handler, T args,
            IOneBotApi api, string rawMsg)
            where T : EventArgs where TResult : BaseQuickOperation
        {
            TResult reply = null;
            foreach (var @delegate in handler.GetInvocationList())
            {
                var func = (EventCallBackHandler<T, TResult>)@delegate;
                try
                {
                    var data = await func(args, api);
                    if (data != default)
                    {
                        reply = data;
                        if (data.Code == 1) break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "消息处理出现未知错误：{Msg}", rawMsg);
                }
            }
            return reply;
        }
        #endregion

        #region 事件类型获取

        /// <summary>
        /// 获取上报事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetBaseEventType(JObject messageJson) =>
            !messageJson.TryGetValue("post_type", out var typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取元事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMetaEventType(JObject messageJson) =>
            !messageJson.TryGetValue("meta_event_type", out var typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取消息事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetMessageType(JObject messageJson) =>
            !messageJson.TryGetValue("message_type", out var typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取请求事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetRequestType(JObject messageJson) =>
            !messageJson.TryGetValue("request_type", out var typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件类型
        /// </summary>
        /// <param name="messageJson">消息Json对象</param>
        private static string GetNoticeType(JObject messageJson) =>
            !messageJson.TryGetValue("notice_type", out var typeJson) ? string.Empty : typeJson.ToString();

        /// <summary>
        /// 获取通知事件子类型
        /// </summary>
        /// <param name="messageJson"></param>
        private static string GetNotifyType(JObject messageJson) =>
            !messageJson.TryGetValue("sub_type", out var typeJson) ? string.Empty : typeJson.ToString();

        #endregion
    }
}