using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Onebot.Models;
using Wuyu.OneBot.Onebot.Models.ApiParams;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Wuyu.OneBot.Interfaces
{
	[JsonNetReturn]
	public interface IOneBotHttpApi: IHttpApi
	{
		[HttpPost("send_msg")]
		public ITask<JObject> SendMsg([JsonNetContent] SendMessageParams sendMessageParams,
			CancellationToken cancellationToken = default);
	}
}