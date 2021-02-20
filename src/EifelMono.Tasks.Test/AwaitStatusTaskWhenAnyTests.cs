using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace EifelMono.Tasks.Test
{
    public class AwaitStatusTaskWhenAllTests
    {
        #region When mutations
        [Fact]
        public async void WhenAll_12()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When());
            Assert.Equal(2, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
        }

        [Fact]
        public async void WhenAll_123()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When(),
                TestTasks.IntAsync(3).When());
            Assert.Equal(3, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
            Assert.Equal(3, result.When3.Result);
        }

        [Fact]
        public async void WhenAll_1234()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When(),
                TestTasks.IntAsync(3).When(),
                TestTasks.IntAsync(4).When());
            Assert.Equal(4, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
            Assert.Equal(3, result.When3.Result);
            Assert.Equal(4, result.When4.Result);
        }

        [Fact]
        public async void WhenAll_12345()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When(),
                TestTasks.IntAsync(3).When(),
                TestTasks.IntAsync(4).When(),
                TestTasks.IntAsync(5).When());
            Assert.Equal(5, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
            Assert.Equal(3, result.When3.Result);
            Assert.Equal(4, result.When4.Result);
            Assert.Equal(5, result.When5.Result);
        }

        [Fact]
        public async void WhenAll_123456()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When(),
                TestTasks.IntAsync(3).When(),
                TestTasks.IntAsync(4).When(),
                TestTasks.IntAsync(5).When(),
                TestTasks.IntAsync(6).When());
            Assert.Equal(6, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
            Assert.Equal(3, result.When3.Result);
            Assert.Equal(4, result.When4.Result);
            Assert.Equal(5, result.When5.Result);
            Assert.Equal(6, result.When6.Result);
        }

        [Fact]
        public async void WhenAll_1234567()
        {
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.IntAsync(1).When(),
                TestTasks.IntAsync(2).When(),
                TestTasks.IntAsync(3).When(),
                TestTasks.IntAsync(4).When(),
                TestTasks.IntAsync(5).When(),
                TestTasks.IntAsync(6).When(),
                TestTasks.IntAsync(7).When());
            Assert.Equal(7, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When1.Result);
            Assert.Equal(2, result.When2.Result);
            Assert.Equal(3, result.When3.Result);
            Assert.Equal(4, result.When4.Result);
            Assert.Equal(5, result.When5.Result);
            Assert.Equal(6, result.When6.Result);
            Assert.Equal(7, result.When7.Result);
        }
        #endregion

        [Fact]
        public async void WhenAll_Test_1()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync().When(),
                TestTasks.IntAsync(1).When());
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds < 100);

            Assert.Equal(2, result.Whens.Length);

            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When2.Result);
        }

        [Fact]
        public async void WhenAll_Test_2()
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.IntAsync(1).When());
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= 1000);

            Assert.Equal(2, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(1, result.When2.Result);
        }

        [Fact]
        public async void WhenAll_Test_3()
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.IntAsync(4711).When());
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= 1000);

            Assert.Equal(7, result.Whens.Length);
            Assert.Empty(result.Canceled);
            Assert.Equal(4711, result.When7.Result);
        }

        [Fact]
        public async void WhenAll_Test_4()
        {
            var stopwatch = Stopwatch.StartNew();

            using var canellationTokenSource1 = new CancellationTokenSource();
            canellationTokenSource1.Cancel();
            using var canellationTokenSource2 = new CancellationTokenSource();
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource1.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource1.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource1.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource2.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource2.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource2.Token).When(),
                TestTasks.IntAsync(4711).When());
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= 1000);

            Assert.Equal(7, result.Whens.Length);
            Assert.Equal(3, result.Canceled.Length);
            Assert.Equal(4711, result.When7.Result);
        }

        [Fact]
        public async void WhenAll_Test_5()
        {
            var stopwatch = Stopwatch.StartNew();

            using var canellationTokenSource1 = new CancellationTokenSource();
            canellationTokenSource1.Cancel();
            using var canellationTokenSource2 = new CancellationTokenSource();
            var result = await WhenAll.AwaitStatusAsync(
                TestTasks.ThrowExceptionAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.ThrowOperationCancelExceptionAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.ThrowTaskCancelExceptionAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource1.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource2.Token).When(),
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1), canellationTokenSource2.Token).When(),
                TestTasks.IntAsync(4711).When());
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= 1000);

            Assert.Equal(7, result.Whens.Length);
            Assert.Equal(3, result.Canceled.Length);
            Assert.Equal(1, result.Faulted.Length);
            Assert.Equal(4711, result.When7.Result);
        }
    }
}
