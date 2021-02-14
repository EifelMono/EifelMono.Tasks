using System;
using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Tasks.Test
{
    public static class TestTasks
    {
        public static Task Test_TaskAsync()
            => Task.Delay(1);

        public static async Task TaskDelayAsync(int millisecondsDelay, CancellationToken cancellationToken)
        {
            await Test_TaskAsync();

            await Task.Delay(millisecondsDelay, cancellationToken).ConfigureAwait(false);
        }

        public static async Task CancellationTokenThrowIfCancellationRequestedAsync(CancellationToken cancellationToken)
        {
            await Test_TaskAsync();

            cancellationToken.ThrowIfCancellationRequested();
        }

        public static async Task TaskCompletionSourceAsync(CancellationToken cancellationToken)
        {
            await Test_TaskAsync();
            var taskCompletionSource = new TaskCompletionSource<int>();
            taskCompletionSource.SetCanceled(cancellationToken);
            await taskCompletionSource.Task;
        }

        public static async Task ThrowException(CancellationToken cancellationToken)
        {
            await Test_TaskAsync();

            throw new Exception("ThrowException");
        }
    }
}
