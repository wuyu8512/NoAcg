using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Internal;
using System;
using System.Net;
using System.Net.Http;
using Wuyu.OneBot.Service;

namespace Wuyu.OneBot
{
    public static class Main
    {
        internal static readonly OneBotOptions Options = new OneBotOptions();

        public static IServiceCollection ConfigureOneBot(this IServiceCollection services,
            Action<OneBotOptions> setupAction = null)
        {
            setupAction?.Invoke(Options);

            services.AddSingleton<EventManager>();
            services.AddHttpApi<IOneBotHttpApi>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseProxy = false
            });
            services.AddSingleton<HttpApi>();

            if (Options.EnableHttpPost)
            {
                if (Options.HttpApi == null)
                {
                    throw new ArgumentException("开启了Http Post但没有传递OneBot的Http Api设置");
                }

                services.ConfigureHttpApi<IOneBotHttpApi>(Options.HttpApi);
            }

            return services;
        }

        public static void UseOneBot(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<OneBotOptions>>();
            if (Options.EnableWebSocketService)
            {
                app.Map(Options.WebSocketUrl, WebSocketService.Map);
                logger.LogInformation("开启了WebSocket服务器，路径为：{Path}", Options.WebSocketUrl);
            }

            if (Options.EnableHttpPost)
            {
                logger.LogInformation("开启了Http Post服务器，路径为：{Path}", "/Message/Post");
            }

            ApplicationLogging.LoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        }
    }
}