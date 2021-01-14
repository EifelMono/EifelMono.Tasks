using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class ObservableValueTests
    {
        [Fact]
        public async void Positive_With_Corret_Value()
        {
            var ov = new ObservableValue<int>(default);
            Assert.Equal(default, ov.Value);
            _ = Task.Run(async () =>
             {
                 await Task.Delay(TimeSpan.FromMilliseconds(100));
                 ov.Value = 1;
             });

            var result = await ov.WaitAsync(1);
            Assert.True(result.AwaitStatus.IsOk());
            Assert.Equal(1, result.Result);
            Assert.Equal(1, ov.Value);
        }


        [Fact]
        public async void Positive_With_Timeout()
        {
            using var ctn = new CancellationTokenNode()
                .WithTimeout(TimeSpan.FromSeconds(1));

            var ov = new ObservableValue<int>(10);
            Assert.Equal(10, ov.Value);

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                // ov.Value = 2;
            });

            var result = await ov.WaitAsync(1, ctn);
            Assert.True(result.AwaitStatus.IsCanceled());
            Assert.False(result.AwaitStatus.IsBranchCanceled());
            Assert.True(result.AwaitStatus.IsTimeoutCanceled());
            Assert.True(result.AwaitStatus.IsNodeCanceled());
            Assert.False(result.AwaitStatus.IsRootCanceled());
            Assert.Equal(10, ov.Value);
        }


        [Fact]
        public async void Positive_With_BranchCanceled()
        {
            using var ctn = new CancellationTokenNode()
                .WithTimeout(TimeSpan.FromSeconds(1));

            var ov = new ObservableValue<int>(10);
            Assert.Equal(10, ov.Value);

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                // ov.Value = 2;
            });

            var result = await ov.WaitAsync(1, ctn);
            Assert.True(result.AwaitStatus.IsCanceled());
            Assert.False(result.AwaitStatus.IsBranchCanceled());
            Assert.True(result.AwaitStatus.IsTimeoutCanceled());
            Assert.True(result.AwaitStatus.IsNodeCanceled());
            Assert.False(result.AwaitStatus.IsRootCanceled());
            Assert.Equal(10, ov.Value);
        }
    }
}
