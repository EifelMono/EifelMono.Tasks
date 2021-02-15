using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EifelMono.Tasks.Test
{
    public class TestPlay
    {

        private ITestOutputHelper Output { get; }

        public TestPlay(ITestOutputHelper output)
        {
            Output = output;
        }

        public async Task<int> TaskIntAsync(int delayInMSec, int returnValue)
        {
            await Task.Delay(delayInMSec);
            return returnValue;
        }

        public async Task<string> TaskStringAsync(int delayInMSec, string returnValue)
        {
            await Task.Delay(delayInMSec);
            return returnValue;
        }
        [Fact]
        public async void Test1()
        {

            var whenAllResult = await WhenAll.AwaitStatusAsync(
                    TaskIntAsync(100, 1),
                    TaskStringAsync(100, "Test1"),
                    TaskIntAsync(1000, 2)
                );
            Assert.Equal(AwaitStatus.Ok, whenAllResult.AwaitStatusTasks[0].AwaitStatus);
            Assert.Equal(AwaitStatus.Ok, whenAllResult.AwaitStatusTasks[1].AwaitStatus);
            Assert.Equal(AwaitStatus.Ok, whenAllResult.AwaitStatusTasks[2].AwaitStatus);

            Assert.Equal(1, whenAllResult.Task1.Result);
            Assert.Equal("Test1", whenAllResult.Task2.Result);
            Assert.Equal(2, whenAllResult.Task3.Result);

            Assert.Equal(1, whenAllResult.Task1.Result);
            Assert.Equal("Test1", whenAllResult.Task2.Result);
            Assert.Equal(2, whenAllResult.Task3.Result);
        }
    }
}
