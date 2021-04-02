using System;
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

        public TweeterMonitorManage(GlobalService globalService,TwitterApi twitterApi,IServiceProvider serviceProvider)
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
        }
    }
}