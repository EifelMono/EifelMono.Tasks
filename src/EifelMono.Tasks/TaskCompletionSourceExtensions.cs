using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class TaskCompletionSourceExtensions
    {
        public static Task<TResult> AsCancellationTask<TResult>(this TaskCompletionSource<TResult> thisValue, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => thisValue.TrySetCanceled());
            return thisValue.Task;
        }
    }
}
