using Microsoft.Extensions.DependencyInjection;
using Sora.Entities.CQCodes;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoAcg.Core
{
	internal class Config
	{
		private readonly InvokeConfig _invokeConfig;
		private readonly JsonSerializerOptions _options;
		private readonly dynamic _eventArgs;

		public Config(IServiceProvider service, dynamic eventArgs)
		{
			_invokeConfig = service.GetService<InvokeConfig>();
			_options = service.GetService<JsonSerializerOptions>();
			_eventArgs = eventArgs;
		}

		public async Task<CQCode> RefreshInvokeConfig()
		{
			var temp =
				JsonSerializer.Deserialize<InvokeConfig>(await File.ReadAllTextAsync("InvokeConfig.json"), _options);
			if (temp == null) return null;
			_invokeConfig.GroupConfig = temp.GroupConfig;
			_invokeConfig.PrivateConfig = temp.PrivateConfig;
			HandleInvokeParams(_invokeConfig);
			return CQCode.CQText("已经成功刷新了调用配置");
		}

		internal static void HandleInvokeParams(InvokeConfig config)
		{
			foreach (var c in config.PrivateConfig.Where(c => c.Param != null))
			{
				c.Param = c.Param.Select(GetValue).ToArray();
			}

			foreach (var c in config.GroupConfig.Where(c => c.Param != null))
			{
				c.Param = c.Param.Select(GetValue).ToArray();
			}
		}

		private static object GetValue(object p)
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