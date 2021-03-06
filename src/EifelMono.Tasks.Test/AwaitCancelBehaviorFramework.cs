﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable xUnit1013 // Public method should be marked as test

namespace EifelMono.Tasks.Test
{
    public class AwaitCancelBehaviorFramework
    {
        // ---------------------------------------------------------------------
        // Exception
        // |
        // +-- OperationCancelException
        //     |
        //     +-- TaskCanceledException
        //
        //
        // OperationCancelException
        // |
        // +-- var cts= new CancellationTokenSource();
        // |   cts.Cancel();
        // |
        // +-- var token= cts.Token;
        // |   token.ThrowIfCancellationRequested() 
        //
        // TaskCanceledException
        // |
        // +-- var cts= new CancellationTokenSource();
        // |   cts.Cancel();
        // |   await Task.Delay(1, cts.Token);
        //
        // ---------------------------------------------------------------------

        #region Await Task (Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_TaskDelayAsync()
        {
            var behaviorOk = false;
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var task = AwaitCancelBehaviorTestTasks.TaskDelayAsync(-1, cancellationTokenSource.Token);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException && ex is OperationCanceledException)
                    behaviorOk = true;
            }
            Assert.True(task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_TaskAsync()
        {
            var behaviorOk = false;
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var task = AwaitCancelBehaviorTestTasks.TaskAsync(cancellationTokenSource.Token);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException && ex is OperationCanceledException)
                    behaviorOk = true;
            }
            Assert.True(task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_ThrowIfCancellationRequestedAsync()
        {
            var behaviorOk = false;
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var task = AwaitCancelBehaviorTestTasks.CancellationTokenThrowIfCancellationRequestedAsync(cancellationTokenSource.Token);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                    behaviorOk = true;
            }
            Assert.True(task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_TaskCompletionSourceAsync()
        {
            var behaviorOk = false;
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var task = AwaitCancelBehaviorTestTasks.TaskCompletionSourceAsync(cancellationTokenSource.Token);
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                    behaviorOk = true;
            }
            Assert.True(task.IsCanceled);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }

        [Fact]
        public async void Behavior_Await_ThrowExceptionAsync()
        {
            var behaviorOk = false;
            using var cancellationTokenSource = new CancellationTokenSource();
            // Cancel direct
            cancellationTokenSource.Cancel();
            var task = AwaitCancelBehaviorTestTasks.ThrowException(cancellationTokenSource.Token);
            try
            {
                await task.ConfigureAwait(true);
            }
            catch
            {
                behaviorOk = true;
            }
            Assert.True(task.IsFaulted);

            Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
        }
        #endregion

        #region Await Task.WhenAll(task1, task2,... (Exception is thrown on Cancel)
        [Fact]
        public async void Behavior_Await_Task_WhenAll_TaskDelayAsync()
        {
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
                var behaviorOk = false;
                var task = AwaitCancelBehaviorTestTasks.TaskLevelAsync(outerLevel, (innerLevel)=>
                {
                    if (outerLevel == innerLevel)
                        throw new OperationCanceledException();
                });
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                        behaviorOk = true;
                }
                Assert.True(task.IsCanceled);

                Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
            }
        }

        [Fact]
        public async void Behavior_Await_LevelTest_Exception()
        {
            for (var outerLevel = 0; outerLevel < 10; outerLevel++)
            {
                var behaviorOk = false;
                var task = AwaitCancelBehaviorTestTasks.TaskLevelAsync(outerLevel, (innerLevel) =>
                {
                    if (outerLevel == innerLevel)
                        throw new Exception();
                });
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is Exception)
                        behaviorOk = true;
                }
                Assert.True(task.IsFaulted);

                Assert.True(behaviorOk, AwaitCancelBehaviorTestTasks.BehaviorText(behaviorOk));
            }
        }
#endregion
    }
}
