using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wuyu.OneBot.Expansion
{
    internal static class TaskExtensions
    {
        public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
            if (await Task.WhenAny(task, delayTask) != task) throw new TimeoutException("The operation has timed out.");
            timeoutCancellationTokenSource.Cancel();
            return await task;
        }
    }
}