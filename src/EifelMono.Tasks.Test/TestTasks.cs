using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public static class TestTasks
    {

        public static async Task DelayAsync(TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
        }

        public static async Task ThrowExceptionAsync(TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
            throw new Exception(nameof(ThrowExceptionAsync));
        }

        public static async Task ThrowOperationCancelExceptionAsync(TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        public static async Task ThrowTaskCancelExceptionAsync(TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            await Task.Run(async () =>
            {
                await Task.Delay(1).ConfigureAwait(false);
            }, cancellationTokenSource.Token);

        }

        public static async Task<int> IntAsync(int result, TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
            return result;
        }

        public static async Task<string> StringAsync(string result, TimeSpan timeSpan = default, CancellationToken cancellationToken = default)
        {
            await Task.Delay(timeSpan, cancellationToken);
            return result;
        }
    }
}
