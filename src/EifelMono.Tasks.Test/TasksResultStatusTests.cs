using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TasksResultStatusTests
    {
        [Fact]
        public async void Test_ResultStatus_Ok()
        {
            using var ctnRoot = new CancellationTokenNode();
            async Task<int> Task1Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                var result = await Task2Async(ctn.Token).AwaitStatusAsync(ctn);
                return result.Result;
            }

            async Task<int> Task2Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                var result = await Task3Async(ctn.Token).AwaitStatusAsync(ctn);
                return result.Result;

            }
            async Task<int> Task3Async(CancellationToken token)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), token)
                    .AwaitStatusAsync();
                return 1;
            }
            var result = await Task1Async(ctnRoot.Token).AwaitStatusAsync(ctnRoot);
            Assert.True(result.IsOk());
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async void Test_ResultStatus_RootCancel()
        {
            using var ctnRoot = new CancellationTokenNode();
            async Task<int> Task1Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                var result = await Task2Async(ctn.Token)
                        .AwaitStatusAsync(ctn);
                result.ThrowIfRootCanceled(ctn);
                return result.Result;
            }

            async Task<int> Task2Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                var result = await Task3Async(ctn.Token)
                          .AwaitStatusAsync(ctn);
                result.ThrowIfRootCanceled(ctn);
                return result.Result;

            }
            async Task<int> Task3Async(CancellationToken token)
            {
                ctnRoot.Root.Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1), token);
                return 1;
            }
            var result = await Task1Async(ctnRoot.Token)
                .AwaitStatusAsync(ctnRoot);
            Assert.True(result.IsRootCanceled());
        }

        [Fact]
        public async void Test_ResultStatus_Root_Task3_Cancel()
        {
            using var ctnRoot = new CancellationTokenNode();
            async Task<int> Task1Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                var result = await Task2Async(ctn.Token)
                        .AwaitStatusAsync(ctn);
                result.ThrowIfRootCanceled(ctn);
                return result.Result;
            }

            async Task<int> Task2Async(CancellationToken token)
            {
                using var ctn = new CancellationTokenNode(token);
                ctn.Branch.Cancel();
                var result = await Task3Async(ctn.Token)
                          .AwaitStatusAsync(ctn);
                result.ThrowIfRootCanceled(ctn);
                if (result.IsBranchCanceled())
                    return 2;
                return result.Result;

            }
            async Task<int> Task3Async(CancellationToken token)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), token);
                return 1;
            }
            var result = await Task1Async(ctnRoot.Token)
                .AwaitStatusAsync(ctnRoot);
            Assert.True(result.IsOk());
            Assert.Equal(2, result.Result);
        }
    }
}
