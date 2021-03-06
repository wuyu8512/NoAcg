﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Wuyu.OneBot.Interfaces;
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
        /// <param name="oneBotApi">对应的OneBot Api接口</param>
        public delegate ValueTask<TResult> EventCallBackHandler<in TEventArgs, TResult>(TEventArgs eventArgs,
            IOneBotApi oneBotApi)
            where TEventArgs : EventArgs where TResult : BaseQuickOperation;

        /// <summary>
        /// Onebot事件回调
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
        /// <param name="rawMsg"></param>
        public async ValueTask<object> Adapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
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
                    case "notice":
                        await NoticeAdapter(messageJson, oneBotApi, rawMsg);
                        break;
                    case "request":
                        RequestAdapter(messageJson, oneBotApi, rawMsg);
                        break;
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

        /// <summary>
        /// 通知事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="oneBotApi">对应事件来源的Api接口</param>
        /// <param name="rawMsg"></param>
        private async ValueTask NoticeAdapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var type = GetNoticeType(messageJson);
            switch (type)
            {
                //群文件上传
                case "group_upload":
                {
                    var fileUpload = messageJson.ToObject<FileUploadEventArgs>();
                    if (OnFileUpload != null && fileUpload != null) await OnFileUpload(fileUpload, oneBotApi);
                    break;
                }
                //群管理员变动
                case "group_admin":
                {
                    var adminChange = messageJson.ToObject<AdminChangeEventArgs>();
                    if (adminChange != null && OnGroupAdminChange != null)
                        await OnGroupAdminChange(adminChange, oneBotApi);
                    break;
                }
                //群成员变动
                case "group_decrease":
                case "group_increase":
                {
                    var groupMemberChange =
                        messageJson.ToObject<GroupMemberChangeEventArgs>();
                    if (groupMemberChange != null && OnGroupMemberChange != null)
                        await OnGroupMemberChange(groupMemberChange, oneBotApi);
                    break;
                }
                //群禁言
                case "group_ban":
                {
                    var groupMute = messageJson.ToObject<GroupBanEventArgs>();
                    if (groupMute != null && OnGroupBan != null) await OnGroupBan(groupMute, oneBotApi);
                    break;
                }
                //好友添加
                case "friend_add":
                {
                    var friendAdd = messageJson.ToObject<FriendAddEventArgs>();
                    if (friendAdd != null && OnFriendAdd != null) await OnFriendAdd(friendAdd, oneBotApi);
                    break;
                }
                //群消息撤回
                case "group_recall":
                {
                    var groupRecall = messageJson.ToObject<GroupRecallEventArgs>();
                    if (groupRecall != null && OnGroupRecall != null) await OnGroupRecall(groupRecall, oneBotApi);
                    break;
                }
                //好友消息撤回
                case "friend_recall":
                {
                    var friendRecall = messageJson.ToObject<FriendRecallEventArgs>();
                    if (friendRecall != null && OnFriendRecall != null)
                        await OnFriendRecall(friendRecall, oneBotApi);
                    break;
                }
                //群名片变更
                //此事件仅在Go上存在
                case "group_card":
                {
                    var groupCardUpdate = messageJson.ToObject<GroupCardUpdateEventArgs>();
                    if (groupCardUpdate != null && OnGroupCardUpdate != null)
                        await OnGroupCardUpdate(groupCardUpdate, oneBotApi);
                    break;
                }
                case "offline_file":
                {
                    var offlineFile = messageJson.ToObject<OfflineFileEventArgs>();
                    if (offlineFile != null && OnOfflineFileEvent != null)
                        await OnOfflineFileEvent(offlineFile, oneBotApi);
                    break;
                }
                case "client_status":
                {
                    var clientStatus = messageJson.ToObject<ClientStatusChangeEventArgs>();
                    if (clientStatus != null && OnClientStatusChange != null)
                        await OnClientStatusChange(clientStatus,
                            oneBotApi);
                    break;
                }
                case "essence":
                {
                    var essenceChange = messageJson.ToObject<EssenceChangeEventArgs>();
                    if (essenceChange != null && OnEssenceChange != null)
                        await OnEssenceChange(essenceChange, oneBotApi);
                    break;
                }
                //通知类事件
                case "notify":
                    var notifyType = GetNotifyType(messageJson);
                    switch (notifyType)
                    {
                        case "poke": //戳一戳
                        {
                            var pokeEvent = messageJson.ToObject<PokeEventArgs>();
                            if (pokeEvent == null) break;
                            if (pokeEvent.GroupId == default)
                            {
                                if (OnGroupPokeEvent != null) await OnGroupPokeEvent(pokeEvent, oneBotApi);
                            }
                            else if (OnFriendPokeEvent != null) await OnFriendPokeEvent(pokeEvent, oneBotApi);

                            break;
                        }
                        case "lucky_king": //运气王
                        {
                            var luckyEvent = messageJson.ToObject<LuckyKingEventArgs>();
                            if (luckyEvent != null && OnLuckyKingEvent != null)
                                await OnLuckyKingEvent(luckyEvent, oneBotApi);
                            break;
                        }
                        case "honor":
                        {
                            var honorEvent = messageJson.ToObject<HonorEventArgs>();
                            if (honorEvent != null && OnHonorEvent != null)
                                await OnHonorEvent(honorEvent, oneBotApi);
                            break;
                        }
                        default:
                            _logger.LogWarning("[Notify]接收到未知事件[{NotifyType}]", notifyType);
                            break;
                    }

                    break;
                default:
                    _logger.LogWarning("[Notice]接收到未知事件[{NoticeType}]", type);
                    break;
            }
        }

        /// <summary>
        /// 请求事件处理和分发
        /// </summary>
        /// <param name="messageJson">消息</param>
        /// <param name="oneBotApi">对应事件来源的Api接口</param>
        /// <param name="rawMsg"></param>
        private async void RequestAdapter(JObject messageJson, IOneBotApi oneBotApi, string rawMsg)
        {
            var type = GetRequestType(messageJson);
            switch (type)
            {
                //好友请求事件
                case "friend":
                {
                    var friendRequest = messageJson.ToObject<FriendRequestEventArgs>();
                    if (friendRequest != null && OnFriendRequest != null)
                        await OnFriendRequest(friendRequest, oneBotApi);
                    break;
                }
                //群组请求事件
                case "group":
                {
                    if (messageJson.TryGetValue("sub_type", out var sub) && sub.ToString().Equals("notice"))
                    {
                        _logger.LogWarning("[Request]收到notice消息类型，不解析此类型消息");
                        break;
                    }

                    var groupRequest = messageJson.ToObject<GroupRequestEventArgs>();
                    if (groupRequest != null && OnGroupRequest != null) await OnGroupRequest(groupRequest, oneBotApi);
                    break;
                }
                default:
                    _logger.LogWarning("[Request]接收到未知事件[{Type}]", type);
                    break;
            }
        }

        #endregion

        #region 事件处理

        private async ValueTask InvokeEvent<T>(EventCallBackHandler<T> handler, T args, IOneBotApi api, string rawMsg)
            where T : EventArgs
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
            where T : EventArgs where TResult : BaseQuickOperation
        {
            TResult reply = null;
            foreach (var @delegate in handler.GetInvocationList())
            {
                var func = (EventCallBackHandler<T, TResult>) @delegate;
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