using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NoAcg.Monitor;
using NoAcgNew.Core;

namespace NoAcgNew.Services
{
    public class TweeterMonitorManage
    {
        private readonly GlobalService _globalService;
        private readonly TwitterApi _twitterApi;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, TweeterMonitor> _tweeterMonitors = new();

        public TweeterMonitorManage(GlobalService globalService, TwitterApi twitterApi,
            IServiceProvider serviceProvider)
        {
            _globalService = globalService;
            _twitterApi = twitterApi;
            _serviceProvider = serviceProvider;
        }

        public void StartNewMonitor(string name, Action<TweeterMonitor, Tweet> action)
        {
            var client = new WebClient();

            var tweeterMonitor = ActivatorUtilities.CreateInstance<TweeterMonitor>(_serviceProvider, name, _twitterApi);
            tweeterMonitor.NewTweetEvent += action;
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