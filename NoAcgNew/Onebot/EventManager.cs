using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NoAcgNew.Interfaces;
using NoAcgNew.Onebot.Models.EventArgs.MessageEventArgs;
using NoAcgNew.Onebot.Models.EventArgs.MetaEventArgs;
using NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs;
using NoAcgNew.Onebot.Models.QuickOperation;
using NoAcgNew.Onebot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Onebot
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
            where TEventArgs : EventArgs where TResult : BaseQuickOperation;

        public delegate ValueTask<int> EventCallBackHandler<in TEventArgs>(TEventArgs eventArgs, IOneBotApi oneBotApi);

        #endregion

        #region 事件回调

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

        public event EventCallBackHandler<AdminChangeEventArgs> OnGroupAdminChange;

        public event EventCallBackHandler<GroupMemberChangeEventArgs> OnGroupMemberChange;

        public event EventCallBackHandler<GroupBanEventArgs> OnGroupBan;

        public event EventCallBackHandler<FriendAddEventArgs> OnFriendAdd;

        public event EventCallBackHandler<GroupRecallEventArgs> OnGroupRecall;

        public event EventCallBackHandler<FriendRecallEventArgs> OnFriendRecall;

        public event EventCallBackHandler<PokeEventArgs> OnFriendPokeEvent;

        public event EventCallBackHandler<PokeEventArgs> OnGroupPokeEvent;

        public event EventCallBackHandler<LuckyKingEventArgs> OnLuckyKingEvent;

        public event EventCallBackHandler<HonorEventArgs> OnHonorEvent;

        public event EventCallBackHandler<GroupCardUpdateEventArgs> OnGroupCardUpdate;

        public event EventCallBackHandler<OfflineFileEventArgs> OnOfflineFileEvent;

        public event EventCallBackHandler<ClientStatusChangeEventArgs> OnClientStatusChange;

        public event EventCallBackHandler<EssenceChangeEventArgs> OnEssenceChange;

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
                    case "notice":
                        await NoticeAdapter(messageJson, oneBotApi, rawMsg);
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