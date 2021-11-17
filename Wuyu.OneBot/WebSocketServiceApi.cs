using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Enumeration.ApiType;
using Wuyu.OneBot.Expansion;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.ApiParams;
using Wuyu.OneBot.Models.QuickOperation;

namespace Wuyu.OneBot
{
    public class WebSocketServiceApi : IOneBotApi
    {
        private readonly WebSocket _socket;
        private readonly Hashtable _replyTable;
        private readonly ILogger<WebSocketServiceApi> _logger;

        public WebSocketServiceApi(WebSocket socket, ILogger<WebSocketServiceApi> logger)
        {
            _socket = socket;
            _replyTable = Hashtable.Synchronized(new Hashtable());
            _logger = logger;
        }

        internal void OnApiReply(JObject json)
        {
            var guid = json["echo"].ToObject<Guid>();
            _replyTable[guid] = json;
        }

        private async Task<JObject> WaitReply(Guid guid)
        {
            while (true)
            {
                if (_replyTable.ContainsKey(guid))
                {
                    var reply = _replyTable[guid] as JObject;
                    _replyTable.Remove(guid);
                    return reply;
                }

                await Task.Delay(50);
            }
        }

        private async ValueTask<(JObject, ApiStatusType)> SendRequest<T>(T request, CancellationToken cancellationToken,
            bool waitReply = true)
            where T : ApiRequest
        {
            if (_socket.State != WebSocketState.Open) return (null, ApiStatusType.Error);
            var str = JsonConvert.SerializeObject(request);
            var data = Encoding.UTF8.GetBytes(str);
            JObject reply = null;
            try
            {
                await _socket.SendAsync(data, WebSocketMessageType.Text, true, cancellationToken);
                if (waitReply) reply = await WaitReply(request.Echo).WaitAsync(TimeSpan.FromSeconds(30));
            }
            catch (TimeoutException e)
            {
                _logger.LogError(e, "[SendRequest]Api请求等待超时，可能没有执行成功，执行的请求[{Request}]", request.ApiRequestType);
                return (reply, ApiStatusType.TimeOut);
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", request.ApiRequestType);
                return (reply, ApiStatusType.Error);
            }
            catch (WebSocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", request.ApiRequestType);
                return (reply, ApiStatusType.Error);
            }
            catch (TaskCanceledException)
            {
                return (null, ApiStatusType.Cancel);
            }

            // TODO ApiStatusType解析
            return (reply, ApiStatusType.Ok);
        }

        public OneBotApiType GetApiType()
        {
            return OneBotApiType.WebSocket;
        }

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long? groupId,
            IEnumerable<CQCode> message, bool? autoEscape = default, CancellationToken cancellationToken = default)
        {
            return await SendMsg(userId, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(null, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendMsg(long? userId, long? groupId, IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SendMsg] User：{UserId} Group：{GroupId}", userId, groupId);
            var request = new ApiRequest<SendMessageParams>
            {
                ApiRequestType = ApiRequestType.SendMsg,
                ApiParams = new SendMessageParams
                { UserId = userId, GroupId = groupId, Message = message, AutoEscape = autoEscape }
            };

            var (reply, statusType) = await SendRequest(request, cancellationToken);
            var id = -1;
            if (reply?["data"] is JObject data && data.ContainsKey("message_id"))
            {
                id = data["message_id"]?.ToObject<int>() ?? -1;
            }

            return reply == null ? (statusType, 0) : (statusType, id);
        }

        internal async ValueTask<ApiStatusType> HandleQuickOperation<T>(JObject content, T operation,
            CancellationToken cancellationToken = default)
            where T : BaseQuickOperation
        {
            _logger.LogInformation("[HandleQuickOperation]发送快速操作");
            var request = new ApiRequest<JObject>
            {
                ApiParams = new JObject
                {
                    ["context"] = content,
                    ["operation"] = JObject.FromObject(operation)
                },
                ApiRequestType = ApiRequestType.HandleQuickOperation
            };

            // 快速操作没有返回值
            var (_, statusType) = await SendRequest(request, cancellationToken);
            return statusType;
        }
    }
}