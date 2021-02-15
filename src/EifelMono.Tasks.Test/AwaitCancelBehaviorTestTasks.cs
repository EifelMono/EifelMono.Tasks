using System;
using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Tasks.Test
{
    public static class AwaitCancelBehaviorTestTasks
    {
        public static Task AsyncAwaitTaskAsync()
            => Task.Delay(1);

        /// <summary>
        /// throws TaskCanceledException (OperationCanceledException)
        /// </summary>
        public static async Task TaskDelayAsync(int millisecondsDelay, CancellationToken cancellationToken)
        {
            await AsyncAwaitTaskAsync();

            await Task.Delay(millisecondsDelay, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// throws TaskCanceledException (OperationCanceledException)
        /// </summary>
        public static async Task TaskAsync(int millisecondsDelay, CancellationToken cancellationToken)
        {
            await AsyncAwaitTaskAsync();

            // if a Task is canceled,
            // the operation inside is still running!
            await Task.Run(async () =>
            {
                await Task.Delay(10);
            }, cancellationToken);
        }

        /// <summary>
        /// throws OperationCanceledException
        /// </summary>
        public static async Task CancellationTokenThrowIfCancellationRequestedAsync(CancellationToken cancellationToken)
        {
            await AsyncAwaitTaskAsync();

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// throws OperationCanceledException
        /// </summary>
        public static async Task TaskCompletionSourceAsync(CancellationToken cancellationToken)
        {
            await AsyncAwaitTaskAsync();

            var taskCompletionSource = new TaskCompletionSource<int>();
            var cancellationTokenRegistration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            try
            {
                await taskCompletionSource.Task;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
            }
        }

        public static async Task ThrowException(CancellationToken cancellationToken)
        {
            await AsyncAwaitTaskAsync();

            throw new Exception("ThrowException");
        }


        public static async Task TaskLevelAsync(int level, Action<int> levelAction)
        {
            await AsyncAwaitTaskAsync();

            levelAction?.Invoke(level);
            await TaskLevelAsync(level++, levelAction);
        }

        public static string BehaviorText(bool behaviorOk)
        {
            var text = behaviorOk ? "Ok" : "not Ok";
            return $"This the current behavior is {text})";
        }
    }
}
