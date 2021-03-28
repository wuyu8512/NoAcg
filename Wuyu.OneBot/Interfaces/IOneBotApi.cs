using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wuyu.OneBot.Enumeration.ApiType;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Interfaces
{
	public interface IOneBotApi
	{
		public OneBotApiType GetApiType();

		public ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long groupId,
			IEnumerable<CQCode> message,
			bool autoEscape = false,
			CancellationToken cancellationToken = default
			);
		
		public ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId,
			IEnumerable<CQCode> message,
			bool autoEscape = false,
			CancellationToken cancellationToken = default
		);
	}
}
