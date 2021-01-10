using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskResultStatusOkTests
    {
        private static async Task<int> TestTaskAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
            return 1;
        }

        [Fact]
        public async void Await_Task_throws_an_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            try
            {
                await task;
                Assert.Equal(1, task.Result);
                Assert.True(true, "This is wrong");
            }
            catch (Exception)
            {
                Assert.True(false, "This is wrong");
            }
        }

        [Fact]
        public async void Await_TaskWhenAny_doesnot_throw_an_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(1, resultTask.Result);
         
            Assert.Equal(task, resultTask);
            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(1, task.Result);
        }

        [Fact]
        public async void Await_Task_with_ResultStatus_and_no_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token)
                .ResultStatusAsync();
            var result = await task;
            Assert.True(result.IsOk());
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async void Await_TaskWhenAny_with_ResultStatus_and_get_StatusResult()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
            Assert.True(task.IsCompletedSuccessfully);
            var result = task.ResultStatus();
            Assert.True(result.IsOk());
            Assert.Equal(1, result.Result);
        }
    }
}
