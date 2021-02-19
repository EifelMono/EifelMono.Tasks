using System;
using System.Diagnostics;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class AwaitStatusTaskWhenAllTests
    {

        [Fact]
        public async void When_1()
        {
            var stopwatch = Stopwatch.StartNew();
            var resultb = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync().When(),
                TestTasks.IntAsync(1).When());
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds < 1000);

            Assert.Empty(resultb.Canceled);
            Assert.Equal(1, resultb.When2.Result);
        }

        [Fact]
        public async void When_2()
        {
            var stopwatch = Stopwatch.StartNew();

            var resultb = await WhenAll.AwaitStatusAsync(
                TestTasks.DelayAsync(TimeSpan.FromSeconds(1)).When(),
                TestTasks.IntAsync(1).When());
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > 1000);

            Assert.Empty(resultb.Canceled);
            Assert.Equal(1, resultb.When2.Result);
        }
    }
}
