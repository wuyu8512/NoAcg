using Core.Function;
using Microsoft.Extensions.DependencyInjection;
using NoAcg.Function;
using Sora.Entities.CQCodes;
using System;
using System.Linq;

namespace NoAcg.Core
{
	class Image
	{
		private readonly Yande _yande;
		private readonly dynamic _eventArgs;

		public Image(IServiceProvider service, dynamic eventArgs)
		{
			_yande = service.GetService<Yande>();
			_eventArgs = eventArgs;
		}

		public CQCode GetHotImg(int rating = 7, bool 发送提示 = true)
		{
			if (发送提示) _eventArgs.Reply(CQCode.CQText("少女祈祷中..."));
			try
			{
				var data = _yande.GetHotImg(out string imgRating, rating);
				return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
			}
			catch (Exception e)
			{
				return CQCode.CQText("请求发生了错误：\n" + e.Message);
			}
		}

		public object GetCosHot()
		{
			if (BiliBili.GetCosHot(out string[] urls))
			{
				return urls.Select(url => CQCode.CQImage(url));
			}
			else return CQCode.CQText("获取失败");
		}
	}
}
