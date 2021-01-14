using System;
using System.Threading;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskTests
    {
        [Theory]
        [InlineData(Globals.CancelMode.SourceCancelDirect)]
        [InlineData(Globals.CancelMode.SourceCancel)]
        [InlineData(Globals.CancelMode.TokenThrow)]
        [InlineData(Globals.CancelMode.ThrowException)]
        public async void Task_await_with_AwaitStatusAsync(Globals.CancelMode cancelMode)
        {
            using var cts = new CancellationTokenSource();
            try
            {
                if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                    cts.Cancel();

                var result = await Globals.TestTaskDelayAsync(cancelMode, cts)
                        .AwaitStatusAsync().ConfigureAwait(false);
                switch (cancelMode)
                {
                    case Globals.CancelMode.SourceCancelDirect:
                    case Globals.CancelMode.SourceCancel:
                    case Globals.CancelMode.TokenThrow:
                        Assert.Equal(AwaitStatus.Canceled, result.AwaitStatus);
                        break;
                    case Globals.CancelMode.ThrowException:
                        Assert.Equal(AwaitStatus.Faulted, result.AwaitStatus);
                        break;
                }
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.ToString());
            }
        }
    }
}
