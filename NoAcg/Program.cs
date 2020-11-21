using Fleck;
using NoAcg.Function;
using Sora.Entities.CQCodes;
using Sora.Server;
using Sora.Tool;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Tool.Common;
using Sora.EventArgs.SoraEvent;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace NoAcg
{
	class Program
	{
		static async Task Main(string[] args)
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			};
			options.Converters.Add(new Common.DatetimeJsonConverter());
			InvokeConfig invokeConfig;
			if (!File.Exists("InvokeConfig.json"))
			{
				invokeConfig = new InvokeConfig
				{
					GroupConfig = new List<InvokeItem>
					{
						new InvokeItem()
						{
							Text = "热门图片",
							ClassName = "NoAcg.Core.Image",
							Method = "GetHotImg",
							Param = new object[]{7,true}
						},
						new InvokeItem()
						{
							Text = "热门Cos",
							ClassName = "NoAcg.Core.Image",
							Method = "GetCosHot",
							Param = null
						}
					},
					PrivateConfig = new List<InvokeItem>
					{
						new InvokeItem()
						{
							Text = "热门图片",
							ClassName = "NoAcg.Core.Image",
							Method = "GetHotImg",
							Param = new object[]{7,false}
						}
					}
				};
				await File.WriteAllTextAsync("InvokeConfig.json", JsonSerializer.Serialize(invokeConfig, options));
				ConsoleLog.Info("NoACG", "没有找到InvokeConfig.json，已经生成默认文件");
			}
			else
			{
				invokeConfig = JsonSerializer.Deserialize<InvokeConfig>(await File.ReadAllTextAsync("InvokeConfig.json"), options);
				ConsoleLog.Info("NoACG", "已经读取配置文件InvokeConfig.json");
			}
			HandleInvokeParams(invokeConfig);

			AppConfig appConfig;
			if (!File.Exists("AppConfig.json"))
			{
				appConfig = new AppConfig
				{
					ServerConfig = new ServerConfig { Port = 9200, Location = "0.0.0.0", ApiTimeOut = 3000 },
					YandeConfig = new YandeConfig { Proxy = new WebProxy("127.0.0.1:2802"), Timeout = 10000 }
				};
				await File.WriteAllTextAsync("AppConfig.json", JsonSerializer.Serialize(appConfig, options));
				ConsoleLog.Info("NoACG", "AppConfig.json，已经生成默认文件");
			}
			else
			{
				appConfig = JsonSerializer.Deserialize<AppConfig>(await File.ReadAllTextAsync("AppConfig.json"), options);
				ConsoleLog.Info("NoACG", "已经读取配置文件AppConfig.json");
			}

			ConsoleLog.SetLogLevel(LogLevel.Debug);
			Yande yande = new Yande(appConfig.YandeConfig);
			//初始化服务器实例
			SoraWSServer server = new SoraWSServer(appConfig.ServerConfig);

			//setup our DI
			var serviceProvider = new ServiceCollection()
				.AddSingleton(yande)
				.AddSingleton(server)
				.BuildServiceProvider();

			bool isConnected = false;
			server.Event.OnClientConnect += async (sender, eventArgs) =>
			{
				isConnected = true;
				var (apiStatus, nick) = await eventArgs.SoraApi.GetLoginUserName();
			};
			//群消息接收回调
			server.Event.OnGroupMessage += async (sender, eventArgs) =>
			{
				if (isConnected) await HandleMessage(serviceProvider, invokeConfig.GroupConfig, eventArgs);
			};
			server.Event.OnPrivateMessage += async (sender, eventArgs) =>
			{
				if (isConnected) await HandleMessage(serviceProvider, invokeConfig.PrivateConfig, eventArgs);
			};
			//启动服务器
			await server.StartServer();
		}
		static async Task HandleMessage(IServiceProvider serviceProvider, List<InvokeItem> config, dynamic eventArgs)
		{
			foreach (var item in config)
			{
				if (IsMatch(item, eventArgs.Message.RawText))
				{
					var t = Type.GetType(item.ClassName);
					ComObject obj = new ComObject(t, serviceProvider, eventArgs);
					var result = obj.InvokeMethod(item.Method, GetParam(item, eventArgs.Message.RawText));
					ConsoleLog.Debug("NoACG", "正在回复消息：" + eventArgs.Message.RawText);
					await eventArgs.Reply(result);
					if (item.Intercept) break;
				}
			}
		}
		static bool IsMatch(InvokeItem config, string rawText)
		{
			if (config.MatchMode == "FullText") return rawText.Equals(config.Text, StringComparison.OrdinalIgnoreCase);
			if (config.MatchMode == "Regex") return Regex.IsMatch(rawText, config.Text);
			return false;
		}
		static object[] GetParam(InvokeItem config, string rawText)
		{
			if (config.MatchMode == "FullText") return config.Param;
			else if (config.MatchMode == "Regex")
			{
				List<object> result = new List<object>();
				foreach (var item in config.Param)
				{
					if (item is string str)
					{
						result.Add(Regex.Replace(rawText, config.Text, str));
					}
					else result.Add(item);
				}
				return result.ToArray();
			}
			return null;
		}
		static void HandleInvokeParams(InvokeConfig config)
		{
			foreach (var c in config.PrivateConfig)
			{
				if (c.Param != null) c.Param = c.Param.Select(p => GetValue(p)).ToArray();

			}
			foreach (var c in config.GroupConfig)
			{
				if (c.Param != null) c.Param = c.Param.Select(p => GetValue(p)).ToArray();
			}
		}
		static object GetValue(object p)
		{
			if (p is JsonElement element)
			{
				switch (element.ValueKind)
				{
					case JsonValueKind.True: return true;
					case JsonValueKind.False: return false;
					case JsonValueKind.Number: return element.GetInt32();
					case JsonValueKind.Null: return null;
					case JsonValueKind.String: return element.GetString();
					case JsonValueKind.Undefined:
						break;
					case JsonValueKind.Object:
						break;
					case JsonValueKind.Array:
						break;
				}
			}
			return p;
		}
	}
}