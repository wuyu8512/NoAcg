using System;
using Microsoft.Extensions.Logging;
using Wuyu.Tool.Expansion;

namespace NoAcgNew.Core
{
    public class TweeterMonitor : Wuyu.Tool.Common.Monitor
    {
        private readonly TwitterApi _twitter;
        private readonly string _userId;
        private readonly ILogger<TweeterMonitor> _logger;
        public string Name { get; }

        private DateTime? _lastDateTime;

        public event Action<TweeterMonitor, Tweet> NewTweetEvent;

        public TweeterMonitor(string name, TwitterApi twitter,ILogger<TweeterMonitor> logger) : base(name)
        {
            _logger = logger;
            Name = name;
            _twitter = twitter;
            NewTweetEvent += (sender, tweet) => logger.LogDebug("{Name}有新的推文了", name);
            _userId = _twitter.GetUserID(name);
            logger.LogDebug("本次监控用户ID为：{UserId}", _userId);
        }

        protected override void Handle()
        {
            if (_userId.IsNull()) return;
            var list = _twitter.GetTweets(_userId);

            _logger.LogDebug("本次{Name}推文数量：{Length}", Name, list.Length);
            if (list.Length == 0)
            {
                return;
            }

            if (_lastDateTime == null)
            {
                _lastDateTime = list[0].CreatTime;
                return;
            }

            foreach (var tweet in list)
            {
                if (tweet.CreatTime <= _lastDateTime) break;
                NewTweetEvent?.Invoke(this, tweet);
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
            if (NewTweetEvent == null) return;
            var invocationList = this.NewTweetEvent.GetInvocationList();
            foreach (Delegate @delegate in invocationList)
            {
                NewTweetEvent -= @delegate as Action<TweeterMonitor, Tweet>;
            }
        }
    }
}