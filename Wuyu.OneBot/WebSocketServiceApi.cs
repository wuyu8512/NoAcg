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

namespace Wuyu.OneBot
{
    public class WebSocketServiceApi : IOneBotApi
    {
        private readonly WebSocket _socket;
        private readonly Hashtable _replayTable;
        private readonly ILogger<WebSocketServiceApi> _logger;

        public WebSocketServiceApi(WebSocket socket, ILogger<WebSocketServiceApi> logger)
        {
            _socket = socket;
            _replayTable = Hashtable.Synchronized(new Hashtable());
            _logger = logger;
        }

        internal void OnApiReplay(JObject json)
        {
            var guid = json["echo"].ToObject<Guid>();
            _replayTable[guid] = json;
        }

        private async Task<JObject> WaitReplay(Guid guid)
        {
            while (true)
            {
                if (_replayTable.ContainsKey(guid))
                {
                    var replay = _replayTable[guid] as JObject;
                    _replayTable.Remove(guid);
                    return replay;
                }

                await Task.Delay(50);
            }
        }

        private async ValueTask<(JObject, ApiStatusType)> SendRequest<T>(T request, CancellationToken cancellationToken)
            where T : ApiRequest
        {
            if (_socket.State != WebSocketState.Open) return (null, ApiStatusType.Error);;
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request, Formatting.None));
            JObject replay = null;
            try
            {
                await _socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true,
                    cancellationToken);
                replay = await WaitReplay(request.Echo).WaitAsync(TimeSpan.FromSeconds(30));
            }
            catch (TimeoutException e)
            {
                _logger.LogError(e, "[SendRequest]Api请求等待超时，可能没有执行成功，执行的请求[{Request}]", request.ApiRequestType);
                return (replay, ApiStatusType.TimeOut);
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", request.ApiRequestType);
                return (replay, ApiStatusType.Error);
            }
            catch (WebSocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", request.ApiRequestType);
                return (replay, ApiStatusType.Error);
            }
            catch (TaskCanceledException)
            {
                return (null, ApiStatusType.Cancel);
            }

            // TODO ApiStatusType解析
            return (replay, ApiStatusType.Ok);
        }

        public OneBotApiType GetApiType()
        {
            return OneBotApiType.WebSocket;
        }

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long? groupId,
            IEnumerable<CQCode> message, bool autoEscape = false, CancellationToken cancellationToken = default)
        {
            return await SendMsg(userId, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message,
            bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(null, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendMsg(long? userId, long? groupId, IEnumerable<CQCode> message,
            bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            var request = new ApiRequest<SendMessageParams>
            {
                ApiRequestType = ApiRequestType.SendMsg,
                ApiParams = new SendMessageParams
                    {UserId = userId, GroupId = groupId, Message = message, AutoEscape = autoEscape}
            };

            var (replay, statusType) = await SendRequest(request, cancellationToken);
            var id = -1;
            if (replay?["data"] is JObject data && data.ContainsKey("message_id"))
            {
                id = data["message_id"]?.ToObject<int>() ?? -1;
            }

            return replay == null ? (statusType, 0) : (statusType, id);
        }
    }
}