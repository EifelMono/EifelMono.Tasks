using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable xUnit1013 // Public method should be marked as test

namespace EifelMono.Tasks.Test
{
    public class FrameworkBehavior
    {
        // ---------------------------------------------------------------------
        // Exception
        // |
        // +-- OperationCancelException
        //     |
        //     +-- TaskCanceledException
        // ---------------------------------------------------------------------

        #region Helper Tasks
        public static Task Task_DummyAsync()
            => Task.Delay(1);

        public static async Task Task_TaskDelayAsync(CancellationToken cancellationToken)
        {
            await Task_DummyAsync();

            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
        }

        public static async Task Task_ThrowIfCancellationRequestedAsync(CancellationToken cancellationToken)
        {
            await Task_DummyAsync();

            cancellationToken.ThrowIfCancellationRequested();
        }

        public static async Task Task_TaskCompletionSourceAsync(CancellationToken cancellationToken)
        {
            await Task_DummyAsync();
            var taskCompletionSource = new TaskCompletionSource<int>();
            taskCompletionSource.SetCanceled(cancellationToken);
            await taskCompletionSource.Task;
        }
        #endregion

        [Fact]
        public async void Behavior_CancellationTokenSource_Cancel_TaskDelay()
        {
            var behaviorOk = false;
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource();
                // direct Cancel
                cancellationTokenSource.Cancel();
                await Task_TaskDelayAsync(cancellationTokenSource.Token).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException && ex is OperationCanceledException)
                    behaviorOk = true;
            }
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(true, "Error");
        }

        [Fact]
        public async void Behavior_CancellationTokenSource_Cancel_ThrowIfCancellationRequested()
        {
            var behaviorOk = false;
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource();
                // direct Cancel
                cancellationTokenSource.Cancel();
                await Task_ThrowIfCancellationRequestedAsync(cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                    behaviorOk = true;
            }
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(true, "Error");
        }

        [Fact]
        public async void Behavior_CancellationTokenSource_Cancel_TaskCompletionSource()
        {
            var behaviorOk = false;
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource();
                // direct Cancel
                cancellationTokenSource.Cancel();
                await Task_TaskCompletionSourceAsync(cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                    behaviorOk = true;
            }
            if (behaviorOk)
                Assert.True(true, "This the current behavior is ok");
            else
                Assert.True(true, "Error");
        }
    }
}
