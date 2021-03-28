using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Internal;
using System;

namespace Wuyu.OneBot
{
	public static class Main
	{
		public static IServiceCollection ConfigureOneBot(this IServiceCollection services)
		{
			services.AddSingleton<EventManager>();
			services.AddHttpApi<IOneBotHttpApi>(o => o.HttpHost = new Uri("http://127.0.0.1:5700"));
			services.AddSingleton<HttpApi>();
			return services;
		}

		public static void UseOneBot(this IApplicationBuilder app)
		{
			// app.Map("/Universal", WebSocketService.Map);
			ApplicationLogging.LoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
		}
	}
}