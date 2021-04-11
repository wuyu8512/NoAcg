using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Enumeration.ApiType;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Interfaces
{
    public interface IOneBotApi
    {
        public OneBotApiType GetApiType();

        public ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long? groupId,
            IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default
        );

        public ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId,
            IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default
        );

        public ValueTask<(ApiStatusType, int)> SendMsg(long? userId, long? groupId,
            IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default);
    }
}