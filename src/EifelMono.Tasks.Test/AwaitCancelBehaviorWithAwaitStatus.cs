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
        public async void Behavior_Await_TaskAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var awaitStatus = await AwaitCancelBehaviorTestTasks.TaskAsync(cancellationTokenSource.Token)
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
            using var cancellationTokenSource1 = new CancellationTokenSource();
            using var cancellationTokenSource2 = new CancellationTokenSource();
            // Cancel 1 direct
            cancellationTokenSource1.Cancel();
          
            var stopwatch = Stopwatch.StartNew();
            var whenAllResult= await WhenAll.AwaitStatusAsync(
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token).When(),
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token).When(),
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token).When()).ConfigureAwait(false);
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > 500);

            Assert.Equal(3, whenAllResult.Whens.Length);

            Assert.Single(whenAllResult.Canceled);

            Assert.True(whenAllResult.When1.Task.IsCanceled);
#if NET5_0
            Assert.True(whenAllResult.When2.Task.IsCompletedSuccessfully);
            Assert.True(whenAllResult.When3.Task.IsCompletedSuccessfully);
#endif
            cancellationTokenSource2.Cancel();
        }
#endregion

#region Await Task.WhenAny(task1, task2,... (no Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_Task_WhenAny_TaskDelayAsync()
        {
            using var cancellationTokenSource1 = new CancellationTokenSource();
            using var cancellationTokenSource2 = new CancellationTokenSource();
            // Cancel 1 direct
            cancellationTokenSource1.Cancel();

            var stopwatch = Stopwatch.StartNew();
            var whenAnyResult = await WhenAny.AwaitStatusAsync(
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(1, cancellationTokenSource1.Token).When(),
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token).When(),
                    AwaitCancelBehaviorTestTasks.TaskDelayAsync(500, cancellationTokenSource2.Token).When()).ConfigureAwait(false);
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 500);

            Assert.Equal(3, whenAnyResult.Whens.Length);

            Assert.Single(whenAnyResult.Canceled);

            Assert.True(whenAnyResult.When1.Task.IsCanceled);
#if NET5_0
            Assert.False(whenAnyResult.When2.Task.IsCompletedSuccessfully);
            Assert.False(whenAnyResult.When3.Task.IsCompletedSuccessfully);
#endif
            cancellationTokenSource2.Cancel();
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
