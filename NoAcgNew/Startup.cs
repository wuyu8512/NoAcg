using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NoAcgNew.Handler;
using NoAcgNew.Services;
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
			services.AddControllers();
			services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "NoAcgNew", Version = "v1" }); });

			services.ConfigureOneBot(o =>
			{
				o.EnableWebSocketClient = Configuration.GetValue<bool>("EnableWebSocketClient");
				o.WebSocketClientUrl = Configuration["OneBotWebSocket:Url"];
				o.EnableWebSocketService = Configuration.GetValue<bool>("EnableWebSocketService");
				o.EnableHttpPost = Configuration.GetValue<bool>("EnableHttpPost");
				o.HttpApi = Configuration.GetSection("OneBotHttpApi");
			});
			services.AddSingleton<GlobalService>();
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

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseOneBot();
			ActivatorUtilities.CreateInstance<MessageHandler>(app.ApplicationServices);
			ActivatorUtilities.CreateInstance<ImageMsgHandler>(app.ApplicationServices);
			ActivatorUtilities.CreateInstance<TwitterHandler>(app.ApplicationServices);
		}
	}
}