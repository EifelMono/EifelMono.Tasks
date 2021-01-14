using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EifelMono.Tasks.Test
{
    public class SpeedTests
    {
        private ITestOutputHelper Output { get; }
        private int Loops  { get; } = 1000000;

        public SpeedTests(ITestOutputHelper output)
        {
            Output = output;
        }

        public static Task<int> TestTaskSpeedAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);

        [Fact]
        public async void Speed_AsyncAWait_With_Cts()
        {
            var stopwatch = Stopwatch.StartNew();
            using var cts = new CancellationTokenSource();
            for (var i = 0; i < Loops; i++)
            {
            
                var result = await TestTaskSpeedAsync(cts.Token);
                Assert.Equal(1, result);
            }
            stopwatch.Stop();
            // 151 msec
            Output.WriteLine($"{nameof(Speed_AsyncAWait_With_Cts)}= {stopwatch.ElapsedMilliseconds}");
        }


        [Fact]
        public async void Speed_AsyncAWait_With_Cts_ResultStatusAsnc()
        {
            var stopwatch = Stopwatch.StartNew();
            using var cts = new CancellationTokenSource();
            for (var i = 0; i < Loops; i++)
            {
                var result = await TestTaskSpeedAsync(cts.Token).AwaitStatusAsync();
                Assert.Equal(1, result.Result);
            }
            stopwatch.Stop();
            // 654 msec
            Output.WriteLine($"{nameof(Speed_AsyncAWait_With_Cts_ResultStatusAsnc)}= {stopwatch.ElapsedMilliseconds}");
        }


        [Fact]
        public async void Speed_AsyncAWait_With_Ctn()
        {
            var stopwatch = Stopwatch.StartNew();
            using var ctn = new CancellationTokenNode();
            for (var i = 0; i < Loops; i++)
            {
                var result = await TestTaskSpeedAsync(ctn.Token);
                Assert.Equal(1, result);
            }
            stopwatch.Stop();
            // 130 msec
            Output.WriteLine($"{nameof(Speed_AsyncAWait_With_Ctn)}= {stopwatch.ElapsedMilliseconds}");
        }


        [Fact]
        public async void Speed_AsyncAWait_With_Ctn_ResultStatusAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            using var ctn = new CancellationTokenNode();
            for (var i = 0; i < Loops; i++)
            {
                var result = await TestTaskSpeedAsync(ctn.Token).AwaitStatusAsync(ctn);
                Assert.Equal(1, result.Result);
            }
            stopwatch.Stop();
            // 695 msec
            Output.WriteLine($"{nameof(Speed_AsyncAWait_With_Ctn_ResultStatusAsync)}= {stopwatch.ElapsedMilliseconds}");
        }

        [Fact]
        public void Speed_CancellationTokenSource()
        {
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < Loops; i++)
            {
                using var ctn = new CancellationTokenSource();
            }
            stopwatch.Stop();
            // 22 msec
            Output.WriteLine($"{nameof(Speed_CancellationTokenSource)}= {stopwatch.ElapsedMilliseconds}");
        }

        [Fact]
        public void Speed_CancellationTokenNode()
        {
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < Loops; i++)
            {
                using var ctn = new CancellationTokenNode();
            }
            stopwatch.Stop();
            // 1319 msec
            Output.WriteLine($"{nameof(Speed_CancellationTokenNode)}= {stopwatch.ElapsedMilliseconds}");
        }
    }
}
