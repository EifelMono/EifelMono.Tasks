using System;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class TasksExtensions
    {
        #region ResultStatusOnlyFrom......
        internal static TaskResultStatus
            ResultStatusOnlyFromTask(this Task thisValue)
        {
            var result = thisValue.Status switch
            {
                TaskStatus.Canceled => TaskResultStatus.Canceled,
                TaskStatus.Faulted => TaskResultStatus.Faulted,
                TaskStatus.RanToCompletion => TaskResultStatus.Ok,
                _ => TaskResultStatus.LookAtTaskDotStatusForMore
            };
            if (result.IsFaulted())
            {
                foreach (var exception in thisValue?.Exception.InnerExceptions)
                {
                    if (exception is OperationCanceledException)
                        result.Include(TaskResultStatus.Canceled);
                    if (exception is OperationRootCanceledException)
                        result.Include(TaskResultStatus.RootCanceled);
                }
            }
            return result;
        }

        internal static TaskResultStatus
           ResultStatusOnlyFromCancellationTokenNode(this CancellationTokenNode thisValue, TaskResultStatus taskResultStatus)
        {
            var result = taskResultStatus;
            if (taskResultStatus.IsCanceled())
            {
                if (thisValue.Node.IsCanceled)
                    result = taskResultStatus.Include(TaskResultStatus.NodeCanceled);
                if (thisValue.Root.IsCanceled)
                    result = taskResultStatus.Include(TaskResultStatus.RootCanceled);
                if (thisValue.Branch.IsCanceled)
                    result = taskResultStatus.Include(TaskResultStatus.BranchCanceled);
                if (thisValue.BranchTimeout.IsCanceled)
                    result = taskResultStatus.Include(TaskResultStatus.TimeoutCanceled);
                if (thisValue.BranchExternals.IsCanceled)
                    result = taskResultStatus.Include(TaskResultStatus.ExternalsCanceled);
            }
            return result;
        }
        #endregion

        #region ResultStatus
        public static (TaskResultStatus ResultStatus, Task Task)
            ResultStatus(this Task thisValue)
        {
            var resultStatus = thisValue.ResultStatusOnlyFromTask();
            return (resultStatus, thisValue);
        }

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            ResultStatus<TResult>(this Task<TResult> thisValue)
        {
            var resultStatus = thisValue.ResultStatusOnlyFromTask();
            return (resultStatus, thisValue, resultStatus == TaskResultStatus.Ok ? thisValue.Result : default);
        }

        public static (TaskResultStatus ResultStatus, Task Task)
            ResultStatus(this Task thisValue, CancellationTokenNode cancellationTokenNode)
        {
            var result = thisValue.ResultStatus();
            result.ResultStatus = cancellationTokenNode.ResultStatusOnlyFromCancellationTokenNode(result.ResultStatus);
            return result;
        }

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            ResultStatus<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            var result = thisValue.ResultStatus();
            result.ResultStatus = cancellationTokenNode.ResultStatusOnlyFromCancellationTokenNode(result.ResultStatus);
            return result;
        }

        public static (TaskResultStatus ResultStatus, Task Task)
            ResultStatus(this (TaskResultStatus ResultStatus, Task Task) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            thisValue.ResultStatus = cancellationTokenNode.ResultStatusOnlyFromCancellationTokenNode(thisValue.ResultStatus);
            return thisValue;
        }

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            ResultStatus<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            thisValue.ResultStatus = cancellationTokenNode.ResultStatusOnlyFromCancellationTokenNode(thisValue.ResultStatus);
            return thisValue;
        }
        #endregion

        #region ResultStatusAsync
        public static async Task<(TaskResultStatus ResultStatus, Task Task)>
            ResultStatusAsync(this Task thisValue)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.ResultStatus();
        }

        public static async Task<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)>
            ResultStatusAsync<TResult>(this Task<TResult> thisValue)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            return thisValue.ResultStatus();
        }

        public static async Task<(TaskResultStatus ResultStatus, Task Task)>
            ResultStatusAsync(this Task thisValue, CancellationTokenNode cancellationTokenNode)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.ResultStatus(cancellationTokenNode);
            return (result.ResultStatus, thisValue);
        }

        public static async Task<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)>
            ResultStatusAsync<TResult>(this Task<TResult> thisValue, CancellationTokenNode cancellationTokenNode)
        {
            _ = await Task.WhenAny(thisValue).ConfigureAwait(false);
            var result = thisValue.ResultStatus(cancellationTokenNode);
            return (result.ResultStatus, thisValue, result.ResultStatus.IsOk() ? thisValue.Result : default);
        }
        #endregion

        #region Shorten the ResultStatus query ...

        #region IsOk
        public static bool
            IsOk(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsOk();
        public static bool
            IsOk<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsOk();
        #endregion

        #region IsFaulted
        public static bool
            IsFaulted(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsFaulted();
        public static bool
            IsFaulted<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsFaulted();
        #endregion

        #region IsLookAtTaskDotStatusForMore
        public static bool
            IsLookAtTaskDotStatusForMore(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsLookAtTaskDotStatusForMore();
        public static bool
            IsLookAtTaskDotStatusForMore<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsLookAtTaskDotStatusForMore();
        #endregion

        #region IsCanceled
        public static bool
            IsCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsCanceled();
        public static bool
            IsCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsCanceled();
        #endregion

        #region IsNodeCanceled
        public static bool
            IsNodeCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsNodeCanceled();
        public static bool
            IsNodeCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsNodeCanceled();
        #endregion

        #region IsRootCanceled
        public static bool
            IsRootCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsRootCanceled();
        public static bool
            IsRootCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsRootCanceled();
        #endregion

        #region IsBranchCanceled
        public static bool
            IsBranchCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsBranchCanceled();
        public static bool
            IsBranchCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsBranchCanceled();
        #endregion

        #region IsTimeoutCanceled
        public static bool
            IsTimeoutCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsTimeoutCanceled();
        public static bool
            IsTimeoutCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsTimeoutCanceled();
        #endregion

        #region IsExternalsCanceled
        public static bool
            IsExternalsCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue)
                => thisValue.ResultStatus.IsExternalsCanceled();
        public static bool
            IsExternalsCanceled<TResult>(this (TaskResultStatus ResultStatus, Task Task, TResult Result) thisValue)
                => thisValue.ResultStatus.IsExternalsCanceled();
        #endregion

        #endregion

        #region On ResultState ...

        #region OnResultState
        public static (TaskResultStatus ResultStatus, Task Task)
            OnResultState(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action, TaskResultStatus resultStatus = default)
        {
            if (resultStatus == default || thisValue.ResultStatus.Contains(resultStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnResultState<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action, TaskResultStatus resultStatus = default)
        {
            if (resultStatus == default || thisValue.ResultStatus.Contains(resultStatus))
                action?.Invoke(thisValue);
            return thisValue;
        }
        #endregion

        #region OnOk
        public static (TaskResultStatus ResultStatus, Task Task)
            OnOk(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Ok);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnOk<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Ok);
        #endregion

        #region OnFaulted
        public static (TaskResultStatus ResultStatus, Task Task)
            OnFaulted(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Faulted);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnFaulted<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Faulted);
        #endregion

        #region OnLookAtTaskDotStatusForMore
        public static (TaskResultStatus ResultStatus, Task Task)
            OnLookAtTaskDotStatusForMore(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.LookAtTaskDotStatusForMore);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnLookAtTaskDotStatusForMore<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.LookAtTaskDotStatusForMore);
        #endregion

        #region OnCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Canceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.Canceled);
        #endregion

        #region OnNodeCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnNodeCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.NodeCanceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnNodeCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.NodeCanceled);
        #endregion

        #region OnRootCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnRootCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.RootCanceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnRootCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.RootCanceled);
        #endregion

        #region OnBranchCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnBranchCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.BranchCanceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnBranchCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.BranchCanceled);
        #endregion

        #region OnTimeoutCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnTimeoutCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.TimeoutCanceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnTimeoutCancele<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.TimeoutCanceled);
        #endregion

        #region OnExternalsCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            OnExternalsCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, Action<(TaskResultStatus ResultStatus, Task Task)> action)
                => thisValue.OnResultState(action, TaskResultStatus.ExternalsCanceled);

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            OnExternalsCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, Action<(TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)> action)
                => thisValue.OnResultState(action, TaskResultStatus.ExternalsCanceled);
        #endregion

        #endregion

        #region ThrowIfRootCanceled
        public static (TaskResultStatus ResultStatus, Task Task)
            ThrowIfRootCanceled(this (TaskResultStatus ResultStatus, Task Task) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            if (thisValue.ResultStatus.IsRootCanceled())
                throw new OperationRootCanceledException(cancellationTokenNode.Root.Token);
            return thisValue;
        }

        public static (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result)
            ThrowIfRootCanceled<TResult>(this (TaskResultStatus ResultStatus, Task<TResult> Task, TResult Result) thisValue, CancellationTokenNode cancellationTokenNode)
        {
            if (thisValue.ResultStatus.IsRootCanceled())
                throw new OperationRootCanceledException(cancellationTokenNode.Root.Token);
            return thisValue;
        }
        #endregion

        #region FireAndForget
        public static async void FireAndForget(this Task thisValue)
            => await thisValue.ResultStatusAsync().ConfigureAwait(false);

        public static async void FireAndForget<TResult>(this Task<TResult> thisValue)
            => await thisValue.ResultStatusAsync().ConfigureAwait(false);
        #endregion
    }
}