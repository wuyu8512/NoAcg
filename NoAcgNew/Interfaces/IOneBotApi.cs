using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NoAcgNew.Onebot.Models;
using NoAcgNew.Enumeration.ApiType;
using Enumeration;
using NoAcgNew.Entities.CQCodes;

namespace NoAcgNew.Interfaces
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
