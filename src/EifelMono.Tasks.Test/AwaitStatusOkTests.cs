using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskAwaitStatusOkTests
    {
#pragma warning disable IDE0060 // Remove unused parameter
        private static Task<int> TestTaskAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
#pragma warning restore IDE0060 // Remove unused parameter

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
#if NET5_0
            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(1, task.Result);
#endif
        }

        [Fact]
        public async void Await_Task_with_AwaitStatus_and_no_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token)
                .AwaitStatusAsync();
            var result = await task;
            Assert.True(result.IsOk());
            Assert.Equal(1, result.Result);
            var onCount = 0;
            result.OnAwaitStatus((r) =>
            {
                onCount++;
                if (r.AwaitStatus.IsOk())
                    onCount++;
            }).OnOk((r) => onCount++);
            Assert.Equal(3, onCount);
        }

        [Fact]
        public async void Await_TaskWhenAny_with_AwaitStatus_and_get_StatusResult()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
#if NET5_0
            Assert.True(task.IsCompletedSuccessfully);
#endif
            var result = task.AwaitStatusFromTask();
            Assert.True(result.IsOk());
            Assert.Equal(1, result.Result);
            var onCount = 0;
            result.OnAwaitStatus((r) =>
            {
                onCount++;
                if (r.AwaitStatus.IsOk())
                    onCount++;
            }).OnOk((r) => onCount++);
            Assert.Equal(3, onCount);
        }
    }
}
