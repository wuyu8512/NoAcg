using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NoAcgNew.Handler;
using NoAcgNew.Interfaces;
using NoAcgNew.Internal;
using NoAcgNew.Onebot;
using System;

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

			services.AddSingleton<EventManager>();
			services.AddSingleton<MessageHandler>();
			services.AddHttpApi<IOneBotHttpApi>(o => o.HttpHost = new Uri("http://127.0.0.1:5700"));
			services.AddSingleton<HttpApi>();
			services.AddControllers().AddNewtonsoftJson();
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
			
			// app.Map("/Universal", WebSocketService.Map);
			ApplicationLogging.LoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
			app.ApplicationServices.GetRequiredService<MessageHandler>();
		}
	}
}