using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EifelMono.Tasks;

#pragma warning disable xUnit1013 // Public method should be marked as test

namespace EifelMono.Tasks.Test
{
    public class AwaitCancelBehavior_with_AwaitStatus
    {
        // ---------------------------------------------------------------------
        // No Exceptions
        //      with AwaitStatusAsync
        // ---------------------------------------------------------------------

        #region Await Task (Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_TaskDelayAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await TestTasks.TaskDelayAsync(-1, cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();

            Assert.True(awaitStatus.Task.IsCanceled);
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(false, "Error");
        }

        [Fact]
        public async void Behavior_Await_ThrowIfCancellationRequestedAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await TestTasks.CancellationTokenThrowIfCancellationRequestedAsync(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();

            Assert.True(awaitStatus.Task.IsCanceled);
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(false, "Error");
        }

        [Fact]
        public async void Behavior_Await_TaskCompletionSourceAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await TestTasks.TaskCompletionSourceAsync(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();
            Assert.True(awaitStatus.Task.IsCanceled);
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(false, "Error");
        }

        [Fact]
        public async void Behavior_Await_ThrowExceptionAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await TestTasks.ThrowException(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsFaulted();
            Assert.True(awaitStatus.Task.IsFaulted);
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(false, "Error");
        }
        #endregion

        #region Await Task.WhenAll(task1, task2,... (Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_Task_WhenAll_TaskDelayAsync()
        {
            throw new NotImplementedException("Todo find solution to for WhenAll");
            var behaviorOk = false;
            using var cancellationTokenSource1 = new CancellationTokenSource();
            using var cancellationTokenSource2 = new CancellationTokenSource();
            // Cancel 1 direct
            cancellationTokenSource1.Cancel();
          
            var task1 = TestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token);
            var task2 = TestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token);
            var task3 = TestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                    behaviorOk = true;
            }
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > 500);

            Assert.True(task1.IsCanceled);
            Assert.True(task2.IsCompletedSuccessfully);
            Assert.True(task3.IsCompletedSuccessfully);

            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(true, "Error");
        }
        #endregion

        #region Await Task.WhenAny(task1, task2,... (no Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_Task_WhenAny_TaskDelayAsync()
        {
            throw new NotImplementedException("Todo find solution to for WhenAny");
            var behaviorOk = false;
            using var cancellationTokenSource1 = new CancellationTokenSource();
            using var cancellationTokenSource2 = new CancellationTokenSource();
            // Cancel 1 direct
            cancellationTokenSource1.Cancel();
            var task1 = TestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token);
            var task2 = TestTasks.TaskDelayAsync(-1, cancellationTokenSource2.Token);
            var task3 = TestTasks.TaskDelayAsync(-1, cancellationTokenSource2.Token);
            var resultTask = await Task.WhenAny(task1, task2, task3).ConfigureAwait(false);

            Assert.Equal(resultTask, task1);
            Assert.True(task1.IsCanceled);
            Assert.False(task2.IsCanceled);
            Assert.False(task3.IsCanceled);

            // Cancel 2 later
            cancellationTokenSource2.Cancel();
            await Task.Delay(100).ConfigureAwait(false);
            Assert.True(task2.IsCanceled);
            Assert.True(task3.IsCanceled);

            if (task1.IsCanceled)
                behaviorOk = true;
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(true, "Error");
        }
        #endregion
    }
}
