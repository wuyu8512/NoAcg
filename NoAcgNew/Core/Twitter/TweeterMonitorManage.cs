using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NoAcgNew.Core;
using NoAcgNew.Core.Twitter;

namespace NoAcgNew.Services
{
    public class TweeterMonitorManage
    {
        private readonly ConfigService _globalService;
        private readonly Lazy<TwitterApi> _twitterApi;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, TweeterMonitor> _tweeterMonitors = new();

        public TweeterMonitorManage(ConfigService globalService, Lazy<TwitterApi> twitterApi,
            IServiceProvider serviceProvider)
        {
            _globalService = globalService;
            _twitterApi = twitterApi;
            _serviceProvider = serviceProvider;
        }

        public void StartNewMonitor(string name, Func<TweeterMonitor, Tweet,ValueTask> action)
        {
            var tweeterMonitor = ActivatorUtilities.CreateInstance<TweeterMonitor>(_serviceProvider, name, _twitterApi.Value);
            tweeterMonitor.ClearAllStartEvent();
            tweeterMonitor.OnNewTweetEvent += action;
            tweeterMonitor.Start();
            _tweeterMonitors[name] = tweeterMonitor;
        }

        public void StopMonitor(string name)
        {
            var tweeterMonitor = _tweeterMonitors[name];
            if (tweeterMonitor == null) throw new KeyNotFoundException($"没有名为{name}的监控对象");
            tweeterMonitor.Close();
        }

        public void RemoveMonitor(string name)
        {
            var tweeterMonitor = _tweeterMonitors[name];
            if (tweeterMonitor == null) throw new KeyNotFoundException($"没有名为{name}的监控对象");
            _tweeterMonitors.Remove(name);
        }
    }
}