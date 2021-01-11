using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskResultStatusCancelTests
    {
        private static async Task TestTaskAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
        }

        [Fact]
        public async void Await_Task_throws_an_exception()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = TestTaskAsync(cts.Token);
            try
            {
                await task;
                Assert.True(false, "This is wrong");
            }
            catch (OperationCanceledException)
            {
                Assert.True(true, "This is correct");
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
            cts.Cancel();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public async void Await_Task_with_ResultStatus_and_no_exception()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = TestTaskAsync(cts.Token)
                .ResultStatusAsync();
            var result = await task;
            Assert.True(result.IsCanceled());
            var onCount = 0;
            result.OnResultState((r) =>
            {
                onCount++;
                if (r.ResultStatus.IsCanceled())
                    onCount++;
            }).OnCanceled((r) => onCount++);
            Assert.Equal(3, onCount);
        }

        [Fact]
        public async void Await_TaskWhenAny_with_ResultStatus_and_StatusResult()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
            Assert.True(task.IsCanceled);
            var result = task.ResultStatus();
            Assert.True(result.IsCanceled());
            var onCount = 0;
            result.OnResultState((r) =>
            {
                onCount++;
                if (r.ResultStatus.IsCanceled())
                    onCount++;
            }).OnCanceled((r) => onCount++);
            Assert.Equal(3, onCount);
        }
    }
}
