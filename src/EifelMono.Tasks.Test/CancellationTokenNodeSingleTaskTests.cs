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

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.Branch.Source).AwaitStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.AwaitStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.AwaitStatus.IsCanceled());
                Assert.True(result.AwaitStatus.IsNodeCanceled());

                Assert.False(result.AwaitStatus.IsRootCanceled());
                Assert.True(result.AwaitStatus.IsBranchCanceled());
                Assert.False(result.AwaitStatus.IsTimeoutCanceled());
                Assert.False(result.AwaitStatus.IsExternalsCanceled());
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

            var result = await Globals.TestTaskDelayAsync(cancelMode, ctn.BranchTimeout.Source).AwaitStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.AwaitStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.AwaitStatus.IsCanceled());
                Assert.True(result.AwaitStatus.IsNodeCanceled());

                Assert.False(result.AwaitStatus.IsRootCanceled());
                Assert.False(result.AwaitStatus.IsBranchCanceled());
                Assert.True(result.AwaitStatus.IsTimeoutCanceled());
                Assert.False(result.AwaitStatus.IsExternalsCanceled());
          
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

            var result = await Globals.TestTaskDelayAsync(cancelMode, external1).AwaitStatusAsync(ctn);

            if (cancelMode == Globals.CancelMode.ThrowException)
            {
                Assert.True(result.AwaitStatus.IsFaulted());
            }
            else
            {
                Assert.True(result.AwaitStatus.IsCanceled());
                Assert.True(result.AwaitStatus.IsNodeCanceled());

                Assert.False(result.AwaitStatus.IsRootCanceled());
                Assert.False(result.AwaitStatus.IsBranchCanceled());
                Assert.False(result.AwaitStatus.IsTimeoutCanceled());
                Assert.True(result.AwaitStatus.IsExternalsCanceled());
            }
        }
    }
}
