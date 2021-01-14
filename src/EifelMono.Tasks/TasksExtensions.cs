using System;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class TasksExtensions
    {
        #region AwaitStatusOnlyFromTask......
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
        public static (AwaitStatus AwaitStatus, Task Task)
            AwaitStatusFromTask(this Task thisValue)
        {
            var awaitStatus = thisValue.AwaitStatusOnlyFromTask();
            return (awaitStatus, thisValue);
        }

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            AwaitStatusFromTask<TResult>(this Task<TResult> thisValue)
        {
            var awaitStatus = thisValue.AwaitStatusOnlyFromTask();
            return (awaitStatus, thisValue, awaitStatus.IsOk() ? thisValue.Result : default);
        }

        public static (AwaitStatus AwaitStatus, Task Task)
            AwaitStatusFromTask(this Task thisValue, CancellationTokenNode cancellationTokenNode)
        {
            var result = thisValue.AwaitStatusFromTask();
            result.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(result.AwaitStatus);
            return result;
        }

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            AwaitStatusFromTask<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            var result = thisValue.AwaitStatusFromTask();
            result.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(result.AwaitStatus);
            return result;
        }

        public static (AwaitStatus AwaitStatus, Task Task)
            AwaitStatusFromTask(this (AwaitStatus AwaitStatus, Task Task) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            thisValue.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(thisValue.AwaitStatus);
            return thisValue;
        }

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            AwaitStatusFromTask<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            thisValue.AwaitStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(thisValue.AwaitStatus);
            return thisValue;
        }
        #endregion

        #region AwaitStatusAsync
        public static async Task<(AwaitStatus AwaitStatus, Task Task)>
            AwaitStatusAsync(this Task thisValue)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.AwaitStatusFromTask();
        }

        public static async Task<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)>
            AwaitStatusAsync<TResult>(this Task<TResult> thisValue)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.AwaitStatusFromTask();
        }

        public static async Task<(AwaitStatus AwaitStatus, Task Task)>
            AwaitStatusAsync(this Task thisValue, CancellationTokenNode cancellationTokenNode)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.AwaitStatusFromTask(cancellationTokenNode);
            return (result.AwaitStatus, thisValue);
        }

        public static async Task<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)>
            AwaitStatusAsync<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.AwaitStatusFromTask(cancellationTokenNode);
            return (result.AwaitStatus, thisValue, result.AwaitStatus.IsOk() ? thisValue.Result : default);
        }
        #endregion

        #region Shorten the AwaitStatus query ...

        #region IsOk
        public static bool
            IsOk(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsOk();
        public static bool
            IsOk<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsOk();
        #endregion

        #region IsFaulted
        public static bool
            IsFaulted(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsFaulted();
        public static bool
            IsFaulted<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsFaulted();
        #endregion

        #region IsLookAtTaskDotStatusForMore
        public static bool
            IsLookAtTaskDotStatusForMore(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsLookAtTaskDotStatusForMore();
        public static bool
            IsLookAtTaskDotStatusForMore<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsLookAtTaskDotStatusForMore();
        #endregion

        #region IsCanceled
        public static bool
            IsCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsCanceled();
        public static bool
            IsCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsCanceled();
        #endregion

        #region IsNodeCanceled
        public static bool
            IsNodeCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsNodeCanceled();
        public static bool
            IsNodeCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsNodeCanceled();
        #endregion

        #region IsRootCanceled
        public static bool
            IsRootCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsRootCanceled();
        public static bool
            IsRootCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsRootCanceled();
        #endregion

        #region IsBranchCanceled
        public static bool
            IsBranchCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsBranchCanceled();
        public static bool
            IsBranchCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsBranchCanceled();
        #endregion

        #region IsTimeoutCanceled
        public static bool
            IsTimeoutCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsTimeoutCanceled();
        public static bool
            IsTimeoutCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsTimeoutCanceled();
        #endregion

        #region IsExternalsCanceled
        public static bool
            IsExternalsCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue)
                => thisValue.AwaitStatus.IsExternalsCanceled();
        public static bool
            IsExternalsCanceled<TResult>(this (AwaitStatus AwaitStatus, Task Task, TResult Result) thisValue)
                => thisValue.AwaitStatus.IsExternalsCanceled();
        #endregion

        #endregion

        #region On ResultState ...

        #region OnResultState
        public static (AwaitStatus AwaitStatus, Task Task)
            OnAwaitStatus(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action, AwaitStatus awaitStatus = default)
        {
            if (awaitStatus == default || thisValue.AwaitStatus.Contains(awaitStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnAwaitStatus<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action, AwaitStatus awaitStatus = default)
        {
            if (awaitStatus == default || thisValue.AwaitStatus.Contains(awaitStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }
        #endregion

        #region OnOk
        public static (AwaitStatus AwaitStatus, Task Task)
            OnOk(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Ok);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnOk<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Ok);
        #endregion

        #region OnFaulted
        public static (AwaitStatus AwaitStatus, Task Task)
            OnFaulted(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Faulted);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnFaulted<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Faulted);
        #endregion

        #region OnLookAtTaskDotStatusForMore
        public static (AwaitStatus AwaitStatus, Task Task)
            OnLookAtTaskDotStatusForMore(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.LookAtTaskDotStatusForMore);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnLookAtTaskDotStatusForMore<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.LookAtTaskDotStatusForMore);
        #endregion

        #region OnCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Canceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.Canceled);
        #endregion

        #region OnNodeCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnNodeCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.NodeCanceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnNodeCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.NodeCanceled);
        #endregion

        #region OnRootCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnRootCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.RootCanceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnRootCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.RootCanceled);
        #endregion

        #region OnBranchCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnBranchCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.BranchCanceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnBranchCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.BranchCanceled);
        #endregion

        #region OnTimeoutCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnTimeoutCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.TimeoutCanceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnTimeoutCancele<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.TimeoutCanceled);
        #endregion

        #region OnExternalsCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            OnExternalsCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, Action<(AwaitStatus AwaitStatus, Task Task)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.ExternalsCanceled);

        public static (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)
            OnExternalsCanceled<TResult>(this (AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result) thisValue, Action<(AwaitStatus AwaitStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnAwaitStatus(action, AwaitStatus.ExternalsCanceled);
        #endregion

        #endregion

        #region ThrowIfRootCanceled
        public static (AwaitStatus AwaitStatus, Task Task)
            ThrowIfRootCanceled(this (AwaitStatus AwaitStatus, Task Task) thisValue, CancellationTokenNode cancellationTokenNode)
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