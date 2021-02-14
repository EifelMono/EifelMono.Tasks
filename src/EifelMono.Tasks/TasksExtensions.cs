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
        public static AwaitStatusTask
            AwaitStatusFromTask(this Task thisValue)
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

        public static AwaitStatusTask
            AwaitStatusFromTask(this Task thisValue, CancellationTokenNode cancellationTokenNode)
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

        public static T
            AwaitStatusFromTask<T>(this T thisValue, CancellationTokenNode cancellationTokenNode) where T : AwaitStatusTaskBase
        {
            thisValue.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(thisValue.AwaitStatus);
            return thisValue;
        }
        #endregion

        #region AwaitStatusAsync
        public static async Task<AwaitStatusTask>
            AwaitStatusAsync(this Task thisValue)
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

        public static async Task<AwaitStatusTask>
            AwaitStatusAsync(this Task thisValue, CancellationTokenNode cancellationTokenNode)
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

        #region On ResultState ...

        #region OnResultState
        public static AwaitStatusTask
            OnAwaitStatus(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action, AwaitStatus awaitStatus = default)
        {
            if (awaitStatus == default || thisValue.AwaitStatus.Contains(awaitStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }

        public static AwaitStatusTaskResult<TResult>
            OnAwaitStatus<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action, AwaitStatus awaitStatus = default)
        {
            if (awaitStatus == default || thisValue.AwaitStatus.Contains(awaitStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }
        #endregion

        #region OnOk
        public static AwaitStatusTask OnOk(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Ok);

        public static AwaitStatusTaskResult<TResult> OnOk<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Ok);
        #endregion

        #region OnFaulted
        public static AwaitStatusTask OnFaulted(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Faulted);

        public static AwaitStatusTaskResult<TResult> OnFaulted<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Faulted);
        #endregion

        #region OnLookAtTaskDotStatusForMore
        public static AwaitStatusTask OnLookAtTaskDotStatusForMore(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.LookAtTaskDotStatusForMore);

        public static AwaitStatusTaskResult<TResult> OnLookAtTaskDotStatusForMore<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.LookAtTaskDotStatusForMore);
        #endregion

        #region OnCanceled
        public static AwaitStatusTask OnCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Canceled);

        public static AwaitStatusTaskResult<TResult> OnCanceled<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.Canceled);
        #endregion

        #region OnNodeCanceled
        public static AwaitStatusTask OnNodeCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.NodeCanceled);

        public static AwaitStatusTaskResult<TResult> OnNodeCanceled<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.NodeCanceled);
        #endregion

        #region OnRootCanceled
        public static AwaitStatusTask OnRootCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.RootCanceled);

        public static AwaitStatusTaskResult<TResult> OnRootCanceled<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.RootCanceled);
        #endregion

        #region OnBranchCanceled
        public static AwaitStatusTask OnBranchCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.BranchCanceled);

        public static AwaitStatusTaskResult<TResult> OnBranchCanceled<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.BranchCanceled);
        #endregion

        #region OnTimeoutCanceled
        public static AwaitStatusTask OnTimeoutCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.TimeoutCanceled);

        public static AwaitStatusTaskResult<TResult> OnTimeoutCancele<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.TimeoutCanceled);
        #endregion

        #region OnExternalsCanceled
        public static AwaitStatusTask OnExternalsCanceled(this AwaitStatusTask thisValue, Action<AwaitStatusTask> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.ExternalsCanceled);

        public static AwaitStatusTaskResult<TResult> OnExternalsCanceled<TResult>(this AwaitStatusTaskResult<TResult> thisValue, Action<AwaitStatusTaskResult<TResult>> action)
            => thisValue.OnAwaitStatus(action, AwaitStatus.ExternalsCanceled);
        #endregion

        #endregion

        #region ThrowIfRootCanceled
        public static AwaitStatusTask
            ThrowIfRootCanceled(this AwaitStatusTask thisValue, CancellationTokenNode cancellationTokenNode)
        {
            if (thisValue.AwaitStatus.IsRootCanceled())
                throw new OperationRootCanceledException(cancellationTokenNode.Root.Token);
            return thisValue;
        }

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            ThrowIfRootCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            if (thisValue.AwaitStatus.IsRootCanceled())
                throw new OperationRootCanceledException(cancellationTokenNode.Root.Token);
            return thisValue;
        }
        #endregion

        #region FireAndForget
        public static async void FireAndForget(this Task thisValue)
            => await thisValue.AwaitStatusAsync().ConfigureAwait(false);

        public static async void FireAndForget<TResult>(this Task<TResult> thisValue)
            => await thisValue.AwaitStatusAsync().ConfigureAwait(false);
        #endregion
    }
}