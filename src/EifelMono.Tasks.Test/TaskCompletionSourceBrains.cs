using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskCompletionSourceBrains
    {
        [Fact]
        public async void Task_CancelTestA()
        {
            var tcs = new TaskCompletionSource<int>();

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                tcs.SetCanceled();
            });

            var result = await tcs.Task.AwaitStatusAsync();
            Assert.True(result.AwaitStatus.IsCanceled());
        }
    }
}
