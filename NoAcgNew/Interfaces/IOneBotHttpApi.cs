using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Interfaces;
using NoAcgNew.Onebot.Models;
using NoAcgNew.Onebot.Models.ApiParams;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace NoAcgNew.Interfaces
{
	[JsonNetReturn]
	public interface IOneBotHttpApi: IHttpApi
	{
		[HttpPost("send_msg")]
		public ITask<JObject> SendMsg([JsonNetContent] SendMessageParams sendMessageParams,
			CancellationToken cancellationToken = default);
	}
}