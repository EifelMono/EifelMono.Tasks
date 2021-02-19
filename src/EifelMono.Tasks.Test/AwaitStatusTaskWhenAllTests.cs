using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace EifelMono.Tasks.Test
{
    public class AwaitStatusTaskWhenAllTests
    {

        [Fact]
        public async void When_1()
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
        public async void When_2()
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
        public async void When_3()
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
        public async void When_4()
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
        public async void When_5()
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
