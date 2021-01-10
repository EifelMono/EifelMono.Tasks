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

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.Root.Source).ResultStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.ResultStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.ResultStatus.IsCanceled());
                Assert.True(result.ResultStatus.IsNodeCanceled());

                Assert.True(result.ResultStatus.IsRootCanceled());
                Assert.False(result.ResultStatus.IsBranchCanceled());
                Assert.False(result.ResultStatus.IsTimeoutCanceled());
                Assert.False(result.ResultStatus.IsExternalsCanceled());
            }
        }
    }
}
