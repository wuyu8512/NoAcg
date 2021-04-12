using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Models.QuickOperation;

namespace Wuyu.OneBot.Service
{
    public sealed class WebSocketService : IDisposable
    {
        private const int BufferSize = 1024;
        private bool _disposed;
        private readonly WebSocket _socket;
        private readonly WebSocketServiceApi _api;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly EventManager _eventManager;
        private readonly ILogger<WebSocketService> _logger;

        public WebSocketService(IHostApplicationLifetime applicationLifetime, HttpContext context,
            EventManager eventManager,
            ILogger<WebSocketService> logger)
        {
            _eventManager = eventManager;
            _logger = logger;
            applicationLifetime.ApplicationStopping.Register(Dispose);
            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
            var task = context.WebSockets.AcceptWebSocketAsync();
            task.Wait();
            _socket = task.Result;
            _api = ActivatorUtilities.CreateInstance<WebSocketServiceApi>(context.RequestServices, _socket);

            logger.LogInformation("有新客户端连接了");
        }

        private async ValueTask EchoLoop()
        {
            _eventManager.Connection(_api);
            while (true)
            {
                WebSocketReceiveResult result = null;
                while (result?.CloseStatus == null)
                {
                    var bufferList = new List<byte>();
                    var buffer = WebSocket.CreateServerBuffer(BufferSize);
                    try
                    {
                        result = await _socket.ReceiveAsync(buffer, _cancellationToken.Token);
                    }
                    catch (SocketException e)
                    {
                        _logger.LogWarning(e, "[WebSocketService] {Msg}", e.Message);
                        return;
                    }
                    catch (WebSocketException e)
                    {
                        _logger.LogWarning(e, "[WebSocketService] {Msg}", e.InnerException?.Message);
                        return;
                    }

                    bufferList.AddRange(buffer[..result.Count]);
                    if (!result.EndOfMessage) continue;
                    if (!result.CloseStatus.HasValue)
                    {
                        MessageHandel(bufferList.ToArray());
                    }
                    else
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                        return;
                    }
                }
            }
        }

        private async void MessageHandel(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return;
                var json = JObject.Parse(str);
                if (json.ContainsKey("post_type"))
                {
                    var result = await _eventManager.Adapter(json, _api, str);
                    if (result is BaseQuickOperation operation) await _api.HandleQuickOperation(json, operation);
                }
                else if (json.ContainsKey("echo")) _api.OnApiReply(json);
            }
            catch (JsonReaderException e)
            {
                _logger.LogError(e, "信息解析出现错误：{Msg}", str);
            }
        }

        ~WebSocketService() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _cancellationToken?.Cancel();
                _cancellationToken?.Dispose();
                _socket?.Dispose();
            }

            _disposed = true;
        }

        private static async Task Acceptor(HttpContext context, Func<Task> next)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;
            var h = ActivatorUtilities.CreateInstance<WebSocketService>(context.RequestServices, context);
            await h.EchoLoop();
            h.Dispose();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(Acceptor);
        }
    }
}