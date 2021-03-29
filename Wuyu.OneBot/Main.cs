using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Internal;
using System;
using Wuyu.OneBot.Service;

namespace Wuyu.OneBot
{
	public static class Main
	{
		internal static readonly OneBotOptions Options = new OneBotOptions();
		public static IServiceCollection ConfigureOneBot(this IServiceCollection services,Action<OneBotOptions> setupAction = null)
		{
			setupAction?.Invoke(Options);
			
			services.AddSingleton<EventManager>();
			services.AddHttpApi<IOneBotHttpApi>();
			services.AddSingleton<HttpApi>();
			
			if (Options.EnableHttpPost)
			{
				if (Options.HttpApiHost == null)
				{
					throw new ArgumentException("开启了Http Post但没有传递Http Host");
				}
				services.ConfigureHttpApi<IOneBotHttpApi>(o => o.HttpHost = Options.HttpApiHost);
			}
			
			return services;
		}

		public static void UseOneBot(this IApplicationBuilder app)
		{
			if (Options.EnableWebSocketService)
			{
				app.Map(Options.WebSocketUrl, WebSocketService.Map);
			}
			ApplicationLogging.LoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
		}
	}
}