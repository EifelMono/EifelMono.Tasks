using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable xUnit1013 // Public method should be marked as test

namespace EifelMono.Tasks.Test
{
    public class AwaitCancelBehaviorwithAwaitStatus
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
            var awaitStatus = await AwaitCancelBehaviorTestTasks.TaskDelayAsync(-1, cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();

            Assert.True(awaitStatus.Task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_ThrowIfCancellationRequestedAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await AwaitCancelBehaviorTestTasks.CancellationTokenThrowIfCancellationRequestedAsync(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();

            Assert.True(awaitStatus.Task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_TaskCompletionSourceAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await AwaitCancelBehaviorTestTasks.TaskCompletionSourceAsync(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsCanceled();
            Assert.True(awaitStatus.Task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_ThrowExceptionAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await AwaitCancelBehaviorTestTasks.ThrowException(cancellationTokenSource.Token)
                .AwaitStatusAsync().ConfigureAwait(false);

            var behaviorOk = awaitStatus.AwaitStatus.IsFaulted();
            Assert.True(awaitStatus.Task.IsFaulted);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
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
          
            var task1 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token);
            var task2 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token);
            var task3 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token);
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
#if NET5_0
            Assert.True(task2.IsCompletedSuccessfully);
            Assert.True(task3.IsCompletedSuccessfully);
#endif
            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
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
            var task1 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token);
            var task2 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(-1, cancellationTokenSource2.Token);
            var task3 = AwaitCancelBehaviorTestTasks.TaskDelayAsync(-1, cancellationTokenSource2.Token);
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

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }
#endregion

#region DeepTaskTests
        [Fact]
        public async void Behavior_Await_LevelTest_Cancel()
        {
            for (var outerLevel = 0; outerLevel < 10; outerLevel++)
            {
                var awaitStatus = await AwaitCancelBehaviorTestTasks.TaskLevelAsync(outerLevel, (innerLevel) =>
                {
                    if (outerLevel == innerLevel)
                        throw new OperationCanceledException();
                }).AwaitStatusAsync().ConfigureAwait(false);

                var behaviorOk = awaitStatus.IsCanceled();
                Assert.True(awaitStatus.Task.IsCanceled);

                Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
            }
        }

        [Fact]
        public async void Behavior_Await_LevelTest_Exception()
        {
            for (var outerLevel = 0; outerLevel < 10; outerLevel++)
            {
                var awaitStatus = await AwaitCancelBehaviorTestTasks.TaskLevelAsync(outerLevel, (innerLevel) =>
                {
                    if (outerLevel == innerLevel)
                        throw new Exception();
                }).AwaitStatusAsync().ConfigureAwait(false);

                var behaviorOk = awaitStatus.IsFaulted();
                Assert.True(awaitStatus.Task.IsFaulted);

                Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
            }
        }
#endregion
    }
}
