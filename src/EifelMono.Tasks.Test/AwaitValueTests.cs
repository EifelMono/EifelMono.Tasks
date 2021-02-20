using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class AwaitValueTests
    {
        [Fact]
        public async void Positive_With_Corret_Value()
        {
            var awaitValue = new AwaitValue<int>(default);
            Assert.Equal(default, awaitValue.Value);
            _ = Task.Run(async () =>
             {
                 await Task.Delay(TimeSpan.FromMilliseconds(100));
                 awaitValue.Value = 1;
             });

            var result = await awaitValue.WaitAsync(1);
            Assert.True(result.AwaitStatus.IsOk());
            Assert.Equal(1, result.Result);
            Assert.Equal(1, awaitValue.Value);
        }


        [Fact]
        public async void Positive_With_Timeout()
        {
            using var ctn = new CancellationTokenNode()
                .WithTimeout(TimeSpan.FromMilliseconds(1));

            var awaitValue = new AwaitValue<int>(10);
            Assert.Equal(10, awaitValue.Value);

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                // awaitValue.Value = 2;
            });

            var result = await awaitValue.WaitAsync(1, ctn);
            Assert.True(result.AwaitStatus.IsCanceled());
            Assert.False(result.AwaitStatus.IsBranchCanceled());
            Assert.True(result.AwaitStatus.IsTimeoutCanceled());
            Assert.True(result.AwaitStatus.IsNodeCanceled());
            Assert.False(result.AwaitStatus.IsRootCanceled());
            Assert.Equal(10, awaitValue.Value);
        }


        [Fact]
        public async void Positive_With_BranchCanceled()
        {
            using var ctn = new CancellationTokenNode()
                .WithTimeout(TimeSpan.FromMilliseconds(100));

            var awaitValue = new AwaitValue<int>(10);
            Assert.Equal(10, awaitValue.Value);

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                    // awaitValue.Value = 2;
                });

            var result = await awaitValue.WaitAsync(1, ctn);
            Assert.True(result.AwaitStatus.IsCanceled());
            Assert.False(result.AwaitStatus.IsBranchCanceled());
            Assert.True(result.AwaitStatus.IsTimeoutCanceled());
            Assert.True(result.AwaitStatus.IsNodeCanceled());
            Assert.False(result.AwaitStatus.IsRootCanceled());
            Assert.Equal(10, awaitValue.Value);
        }
    }
}
