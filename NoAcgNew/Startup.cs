using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NoAcgNew.Attributes;
using NoAcgNew.Converter;
using NoAcgNew.Handler;
using NoAcgNew.Services;
using Polly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Wuyu.OneBot;

namespace NoAcgNew
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var setting = new JsonSerializerSettings();
            setting.Converters.Add(new WebProxyJsonConverter());
            JsonConvert.DefaultSettings = new(() => setting);

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "NoAcgNew", Version = "v1" }); });

            services.ConfigureOneBot(o =>
            {
                o.EnableWebSocketClient = Configuration.GetValue<bool>("EnableWebSocketClient");
                o.WebSocketClientUrl = Configuration["OneBotWebSocket:Url"];
                o.EnableWebSocketService = Configuration.GetValue<bool>("EnableWebSocketService");
                o.EnableHttpPost = Configuration.GetValue<bool>("EnableHttpPost");
                o.HttpApi = Configuration.GetSection("OneBotHttpApi");
            });
             
            services.AddSingleton<ConfigService>();
            services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

            HttpMessageHandler configHandler(IServiceProvider s)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy()
                };
                Configuration.GetSection("HttpClient").Bind(handler);
                return handler;
            }

            services.AddHttpClient("default")
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
                .ConfigurePrimaryHttpMessageHandler(configHandler);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NoAcgNew v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
             
            app.UseOneBot();
            Task.Run(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                foreach (var type in assembly.GetTypes())
                {
                    var customAttributes = type.GetCustomAttributes(typeof(HandlerAttribute), false).FirstOrDefault();
                    if (customAttributes != null)
                    {
                        ActivatorUtilities.CreateInstance(app.ApplicationServices, type);
                    }
                }
            });
        }
    }
}