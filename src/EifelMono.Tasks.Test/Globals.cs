using System;
using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Tasks.Test
{
    public static class Globals
    {
        public enum CancelMode
        {
            SourceCancelDirect,
            SourceCancel,
            TokenThrow,
            ThrowException,
        }

        public static async Task TestTaskDelayAsync(CancelMode cancelMode, CancellationTokenSource cancellationTokenSource= default, TimeSpan delayWait= default)
        {
            if (delayWait == default)
                delayWait = TimeSpan.FromSeconds(1);
            switch (cancelMode)
            {
                case CancelMode.SourceCancelDirect:
                case CancelMode.SourceCancel:
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                        switch (cancelMode)
                        {
                            case CancelMode.SourceCancelDirect:
                                break;
                            case CancelMode.SourceCancel:
                                cancellationTokenSource?.Cancel();
                                break;
                        }
                    });
                    await Task.Delay(delayWait, cancellationTokenSource?.Token?? default);
                    break;
                case CancelMode.TokenThrow:
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource?.Token.ThrowIfCancellationRequested();
                    break;
                case CancelMode.ThrowException:
                    throw new Exception(nameof(CancelMode.ThrowException));
            }
        }
    }
}
