using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enumeration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Enumeration.ApiType;
using NoAcgNew.Expansion;
using NoAcgNew.Interfaces;
using NoAcgNew.Internal;
using NoAcgNew.Onebot.Models;
using NoAcgNew.Onebot.Models.ApiParams;

namespace NoAcgNew.Onebot
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
            var str = MessageHelper.ConvertToJson(request);
            var data = Encoding.UTF8.GetBytes(str);
            JObject replay = null;
            try
            {
                await _socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true,
                    cancellationToken);
                replay = await WaitReplay(request.Echo).WaitAsync(TimeSpan.FromSeconds(30));
            }
            catch (TimeoutException e)
            {
                _logger.LogError(e, "[SendRequest]Api请求等待超时，可能没有执行成功，执行的请求[{Request}]", str);
                return (replay, ApiStatusType.TimeOut);
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", str);
                return (replay, ApiStatusType.Error);
            }
            catch (WebSocketException e)
            {
                _logger.LogError(e, "[SendRequest] {Request}", str);
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

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long groupId,
            IEnumerable<CQCode> message, bool autoEscape = false, CancellationToken cancellationToken = default)
        {
            var request = new ApiRequest<SendMessageParams>
            {
                ApiRequestType = ApiRequestType.SendMsg,
                ApiParams = new SendMessageParams
                    {UserId = userId, GroupId = groupId, Message = message, AutoEscape = autoEscape}
            };

            var (replay, statusType) = await SendRequest(request, cancellationToken);
            return replay == null
                ? (statusType, 0)
                : (statusType, replay["data"]?["message_id"]?.ToObject<int>() ?? -1);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message,
            bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            var request = new ApiRequest<SendMessageParams>
            {
                ApiRequestType = ApiRequestType.SendMsg,
                ApiParams = new SendMessageParams
                    {UserId = 0, GroupId = groupId, Message = message, AutoEscape = autoEscape}
            };

            var (replay, statusType) = await SendRequest(request, cancellationToken);
            return replay == null
                ? (statusType, 0)
                : (statusType, replay["data"]?["message_id"]?.ToObject<int>() ?? -1);
        }
    }
}