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
        public async void Task_await_with_ResultStatusAsync(Globals.CancelMode cancelMode)
        {
            using var cts = new CancellationTokenSource();
            try
            {
                if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                    cts.Cancel();

                var result = await Globals.TestTaskDelayAsync(cancelMode, cts)
                        .ResultStatusAsync().ConfigureAwait(false);
                switch (cancelMode)
                {
                    case Globals.CancelMode.SourceCancelDirect:
                    case Globals.CancelMode.SourceCancel:
                    case Globals.CancelMode.TokenThrow:
                        Assert.Equal(TaskResultStatus.Canceled, result.ResultStatus);
                        break;
                    case Globals.CancelMode.ThrowException:
                        Assert.Equal(TaskResultStatus.Faulted, result.ResultStatus);
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
