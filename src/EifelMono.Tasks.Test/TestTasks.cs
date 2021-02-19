using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public static class TestTasks
    {
        public static  async Task DelayAsync(TimeSpan timeSpan= default, CancellationToken cancellationToken= default)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new Exception();
            await Task.Delay(timeSpan, cancellationToken);
        }

        public static async Task<int> IntAsync(int result, TimeSpan timeSpan= default, CancellationToken cancellationToken= default)
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
