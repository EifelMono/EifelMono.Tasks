using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
namespace EifelMono.Tasks.Test
{
    public class CancellationTokenNodeMultiTaskTests
    {
        [Theory]
        [InlineData(Globals.CancelMode.SourceCancelDirect)]
        [InlineData(Globals.CancelMode.SourceCancel)]
        [InlineData(Globals.CancelMode.TokenThrow)]
        [InlineData(Globals.CancelMode.ThrowException)]
        public async void Cancel_ByRoot(Globals.CancelMode cancelMode)
        {
            using var ctn = new CancellationTokenNode();
            if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                ctn.Root.Cancel();

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.Root.Source).AwaitStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.AwaitStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.AwaitStatus.IsCanceled());
                Assert.True(result.AwaitStatus.IsNodeCanceled());

                Assert.True(result.AwaitStatus.IsRootCanceled());
                Assert.False(result.AwaitStatus.IsBranchCanceled());
                Assert.False(result.AwaitStatus.IsTimeoutCanceled());
                Assert.False(result.AwaitStatus.IsExternalsCanceled());
            }
        }
    }
}
