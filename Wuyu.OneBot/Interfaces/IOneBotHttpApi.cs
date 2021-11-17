using Newtonsoft.Json.Linq;
using System.Threading;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using Wuyu.OneBot.Models.ApiParams;

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