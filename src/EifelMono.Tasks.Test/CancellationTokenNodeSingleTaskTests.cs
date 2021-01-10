using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
namespace EifelMono.Tasks.Test
{
    public class CancellationTokenNodeSingleTaskTests
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

        [Theory]
        [InlineData(Globals.CancelMode.SourceCancelDirect)]
        [InlineData(Globals.CancelMode.SourceCancel)]
        [InlineData(Globals.CancelMode.TokenThrow)]
        [InlineData(Globals.CancelMode.ThrowException)]
        public async void Cancel_ByBranch(Globals.CancelMode cancelMode)
        {
            using var ctn = new CancellationTokenNode();
            if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                ctn.Branch.Cancel();

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.Branch.Source).ResultStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.ResultStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.ResultStatus.IsCanceled());
                Assert.True(result.ResultStatus.IsNodeCanceled());

                Assert.False(result.ResultStatus.IsRootCanceled());
                Assert.True(result.ResultStatus.IsBranchCanceled());
                Assert.False(result.ResultStatus.IsTimeoutCanceled());
                Assert.False(result.ResultStatus.IsExternalsCanceled());
            }
        }

        [Theory]
        [InlineData(Globals.CancelMode.SourceCancelDirect)]
        [InlineData(Globals.CancelMode.SourceCancel)]
        [InlineData(Globals.CancelMode.TokenThrow)]
        [InlineData(Globals.CancelMode.ThrowException)]
        public async void Cancel_ByTimeout(Globals.CancelMode cancelMode)
        {
            using var ctn = new CancellationTokenNode()
                .WithTimeout(TimeSpan.FromMilliseconds(100));
            if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                ctn.BranchTimeout.Cancel();

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.BranchTimeout.Source).ResultStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.ResultStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.ResultStatus.IsCanceled());
                Assert.True(result.ResultStatus.IsNodeCanceled());

                Assert.False(result.ResultStatus.IsRootCanceled());
                Assert.False(result.ResultStatus.IsBranchCanceled());
                Assert.True(result.ResultStatus.IsTimeoutCanceled());
                Assert.False(result.ResultStatus.IsExternalsCanceled());
          
            }
        }

        [Theory]
        [InlineData(Globals.CancelMode.SourceCancelDirect)]
        [InlineData(Globals.CancelMode.SourceCancel)]
        [InlineData(Globals.CancelMode.TokenThrow)]
        [InlineData(Globals.CancelMode.ThrowException)]
        public async void Cancel_ByExternals(Globals.CancelMode cancelMode)
        {
            using var external1 = new CancellationTokenSource();
            using var external2 = new CancellationTokenSource();
            using var ctn = new CancellationTokenNode()
                .WithExternals(external1, external2);
            if (cancelMode == Globals.CancelMode.SourceCancelDirect)
                external1.Cancel();

            var result = await Globals.TestTaskDelayAsync(cancelMode, external1).ResultStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.ResultStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.ResultStatus.IsCanceled());
                Assert.True(result.ResultStatus.IsNodeCanceled());

                Assert.False(result.ResultStatus.IsRootCanceled());
                Assert.False(result.ResultStatus.IsBranchCanceled());
                Assert.False(result.ResultStatus.IsTimeoutCanceled());
                Assert.True(result.ResultStatus.IsExternalsCanceled());
            }
        }
    }
}
