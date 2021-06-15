using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wuyu.Tool.Expansion;

namespace NoAcgNew.Core.Twitter
{
    public class TweeterMonitor : Wuyu.Tool.Common.Monitor
    {
        private readonly TwitterApi _twitter;
        private readonly Lazy<string> _userId;
        private readonly ILogger<TweeterMonitor> _logger;
        public string Name { get; }

        private DateTime? _lastDateTime;

        public event Action<TweeterMonitor, Tweet> OnNewTweetEvent;

        public TweeterMonitor(string name, TwitterApi twitter, ILogger<TweeterMonitor> logger) : base(name)
        {
            _logger = logger;
            Name = name;
            _twitter = twitter;
            OnNewTweetEvent += (sender, tweet) => logger.LogDebug("{Name}有新的推文了", name);
            _userId = new Lazy<string>(() =>
            {
                var id = _twitter.GetUserIDAsync(name).Result;
                logger.LogInformation("本次监控用户ID为：{UserId}", id);
                return id;
            });
        }

        protected override async Task Handle()
        {
            if (_userId.Value.IsNull()) return;
            var list = await _twitter.GetTweetsAsync(_userId.Value);
            if (!list.Any())
            {
                return;
            }

            _logger.LogDebug("本次{Name}推文数量：{Length}", Name, list.Length);

            if (_lastDateTime == null)
            {
                _lastDateTime = list[0].CreatTime;
                return;
            }

            foreach (var tweet in list)
            {
                if (tweet.CreatTime <= _lastDateTime) break;
                OnNewTweetEvent?.Invoke(this, tweet);
            }

            _lastDateTime = list[0].CreatTime;
        }

        // private string Translate(string text)
        // {
        // 	StringBuilder sb = new StringBuilder(text);
        // 	sb.Append("\n\n翻译结果：");
        // 	string[] array = text.Split('\n');
        // 	for (int i = 0; i < array.Length; i++)
        // 	{
        // 		if (i <= 1) continue;
        // 		sb.Append("\n" + array[i].Translate());
        // 	}
        // 	return sb.ToString();
        // }

        public void ClearAllNewTweetEvent()
        {
            if (OnNewTweetEvent == null) return;
            var invocationList = this.OnNewTweetEvent.GetInvocationList();
            foreach (Delegate @delegate in invocationList)
            {
                OnNewTweetEvent -= @delegate as Action<TweeterMonitor, Tweet>;
            }
        }
    }
}