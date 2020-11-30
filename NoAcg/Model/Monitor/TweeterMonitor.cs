using Sora.Tool;
using System;

namespace NoAcg.Model.Monitor
{
    public class TweeterMonitor : Tool.Common.Monitor
    {
        private readonly Twitter _twitter;
        private readonly string _userId;
        public string Mark { get; private set; }

        private DateTime? _lastDateTime = null;

        public event Action<TweeterMonitor, Tweet> NewTweetEvent;

        public TweeterMonitor(string mark, Twitter twitter) : base(mark)
        {
            Mark = mark;
            _twitter = twitter;
            NewTweetEvent += (sender, tweet) => ConsoleLog.Debug("NoACG [TweeterMonitor]", $"{Mark}有新的推文了");
            _userId = _twitter.GetUserID(mark);
            ConsoleLog.Debug("NoACG [TweeterMonitor]", $"本次监控用户ID为：{_userId}");
        }

        protected override void Handle()
        {
            Tweet[] list;
            lock (_twitter)
            {
                list = _twitter.GetTweets(_userId);
            }

            ConsoleLog.Debug("NoACG [TweeterMonitor]", $"本次{Mark}推文数量：{list.Length}");
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