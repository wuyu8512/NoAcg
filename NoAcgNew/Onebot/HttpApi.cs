using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Enumeration;
using NoAcgNew.Enumeration.ApiType;
using NoAcgNew.Enumeration.EventParamsType;
using NoAcgNew.Interfaces;
using NoAcgNew.Onebot.Models.ApiParams;

namespace NoAcgNew.Onebot
{
    public class HttpApi: IOneBotApi
    {
        private readonly IOneBotHttpApi _api;

        public HttpApi(IOneBotHttpApi api)
        {
            _api = api;
        }
        
        public OneBotApiType GetApiType()
        {
            return OneBotApiType.Http;
        }

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long groupId, IEnumerable<CQCode> message, bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            var replay = await _api.SendMsg(new SendMessageParams
            {
                UserId = userId,
                GroupId = groupId,
                Message = message,
                AutoEscape = autoEscape,
                MessageType = MessageType.Private
            }, cancellationToken);
            
            // TODO ApiStatusType解析
           return replay == null
               ? (ApiStatusType.Error, 0)
               : (ApiStatusType.Ok, replay["data"]?["message_id"]?.ToObject<int>() ?? -1);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message, bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            var replay = await _api.SendMsg(new SendMessageParams
            {
                GroupId = groupId,
                Message = message,
                AutoEscape = autoEscape,
                MessageType = MessageType.Group
            }, cancellationToken);
            
            // TODO ApiStatusType解析
            return replay == null
                ? (ApiStatusType.Error, 0)
                : (ApiStatusType.Ok, replay["data"]?["message_id"]?.ToObject<int>() ?? -1);
        }
    }
}