using System;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class TasksExtensions
    {
        #region AwaitStatusOnlyFrom......
        internal static AwaitStatus
            AwaitStatusOnlyFromTask(this Task thisValue)
        {
            var result = thisValue.Status switch
            {
                TaskStatus.Canceled => AwaitStatus.Canceled,
                TaskStatus.Faulted => AwaitStatus.Faulted,
                TaskStatus.RanToCompletion => AwaitStatus.Ok,
                _ => AwaitStatus.LookAtTaskDotStatusForMore
            };
            if (result.IsFaulted())
            {
                foreach (var exception in thisValue?.Exception.InnerExceptions)
                {
                    if (exception is OperationCanceledException)
                        result.Include(AwaitStatus.Canceled);
                    if (exception is OperationRootCanceledException)
                        result.Include(AwaitStatus.RootCanceled);
                }
            }
            return result;
        }

        internal static AwaitStatus
           AwaitStatusOnlyFromCancellationTokenNode(this CancellationTokenNode thisValue, AwaitStatus awaitStatus)
        {
            var result = awaitStatus;
            if (awaitStatus.IsCanceled())
            {
                if (thisValue.Node.IsCanceled)
                    result = awaitStatus.Include(AwaitStatus.NodeCanceled);
                if (thisValue.Root.IsCanceled)
                    result = awaitStatus.Include(AwaitStatus.RootCanceled);
                if (thisValue.Branch.IsCanceled)
                    result = awaitStatus.Include(AwaitStatus.BranchCanceled);
                if (thisValue.BranchTimeout.IsCanceled)
                    result = awaitStatus.Include(AwaitStatus.TimeoutCanceled);
                if (thisValue.BranchExternals.IsCanceled)
                    result = awaitStatus.Include(AwaitStatus.ExternalsCanceled);
            }
            return result;
        }
        #endregion

        #region AwaitStatusFromTask
        public static AwaitStatusTask<TTask>
            AwaitStatusFromTask<TTask>(this TTask thisValue) where TTask : Task
        {
            var awaitStatus = thisValue.AwaitStatusOnlyFromTask();
            return new(awaitStatus, thisValue);
        }

        public static AwaitStatusTaskResult<TResult>
            AwaitStatusFromTask<TResult>(this Task<TResult> thisValue)
        {
            var awaitStatus = thisValue.AwaitStatusOnlyFromTask();
            return new(awaitStatus, thisValue, awaitStatus.IsOk() ? thisValue.Result : default);
        }

        public static AwaitStatusTask<TTask>
            AwaitStatusFromTask<TTask>(this TTask thisValue, CancellationTokenNode cancellationTokenNode) where TTask : Task
        {
            var result = thisValue.AwaitStatusFromTask();
            result.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(result.AwaitStatus);
            return result;
        }

        public static AwaitStatusTaskResult<TResult>
            AwaitStatusFromTask<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            var result = thisValue.AwaitStatusFromTask();
            result.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(result.AwaitStatus);
            return result;
        }

        #endregion

        #region AwaitStatusAsync
        public static async Task<AwaitStatusTask<TTask>>
            AwaitStatusAsync<TTask>(this TTask thisValue) where TTask : Task
        { 
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.AwaitStatusFromTask();
        }

        public static async Task<AwaitStatusTaskResult<TResult>>
            AwaitStatusAsync<TResult>(this Task<TResult> thisValue)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.AwaitStatusFromTask();
        }

        public static async Task<AwaitStatusTask<TTask>>
            AwaitStatusAsync<TTask>(this TTask thisValue, CancellationTokenNode cancellationTokenNode) where TTask : Task
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.AwaitStatusFromTask(cancellationTokenNode);
            return new(result.AwaitStatus, thisValue);
        }

        public static async Task<AwaitStatusTaskResult<TResult>>
            AwaitStatusAsync<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.AwaitStatusFromTask(cancellationTokenNode);
            return new(result.AwaitStatus, thisValue, result.AwaitStatus.IsOk() ? thisValue.Result : default);
        }
        #endregion

        #region Shorten the AwaitStatus stuff ...
        public static bool IsOk(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsOk();

        public static bool IsFaulted(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsFaulted();

        public static bool IsLookAtTaskDotStatusForMore(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsLookAtTaskDotStatusForMore();

        public static bool IsCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsCanceled();

        public static bool IsNodeCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsNodeCanceled();

        public static bool IsRootCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsRootCanceled();

        public static bool IsBranchCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsBranchCanceled();

        public static bool IsTimeoutCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsTimeoutCanceled();

        public static bool IsExternalsCanceled(this AwaitStatusTaskBase thisValue)
            => thisValue.AwaitStatus.IsExternalsCanceled();
        #endregion

        #region On AwaitStatus ...
        public static T OnAwaitStatus<T>(this T thisValue, Action<T> action, AwaitStatus awaitStatus = default) where T : AwaitStatusTaskBase
        {
            if (awaitStatus == default || thisValue.AwaitStatus.Contains(awaitStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }

        public static T OnOk<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.Ok);

        public static T OnFaulted<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.Faulted);

        public static T OnLookAtTaskDotStatusForMore<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.LookAtTaskDotStatusForMore);

        public static T OnCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.Canceled);

        public static T OnNodeCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.NodeCanceled);

        public static T OnRootCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.RootCanceled);

        public static T OnBranchCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.BranchCanceled);

        public static T OnTimeoutCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.TimeoutCanceled);

        public static T OnExternalsCanceled<T>(this T thisValue, Action<T> action) where T : AwaitStatusTaskBase
            => thisValue.OnAwaitStatus(action, AwaitStatus.ExternalsCanceled);
        #endregion

        #region ThrowIfRootCanceled
        public static T
            ThrowIfRootCanceled<T>(this T thisValue, CancellationTokenNode cancellationTokenNode) where T : AwaitStatusTaskBase
        {
            if (thisValue.AwaitStatus.IsRootCanceled())
                throw new OperationRootCanceledException(cancellationTokenNode.Root.Token);
            return thisValue;
        }
        #endregion

        #region FireAndForget
        public static async void FireAndForget<T>(this T thisValue) where T : Task
            => await thisValue.AwaitStatusAsync().ConfigureAwait(false);
        #endregion
    }
}