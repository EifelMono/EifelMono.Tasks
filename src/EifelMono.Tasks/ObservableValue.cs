using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public class ObservableValue<T> : IDisposable
    {
        public ObservableValue()
        {
        }

        public ObservableValue(T value)
        {
            _value = value;
        }

        public void Dispose()
        {
            OnChanged.Dispose();
        }

        protected object _lockValue = new();

        protected T _value;
        public T Value
        {
            get
            {
                lock (_lockValue)
                    return _value;
            }
            set
            {
                lock (_lockValue)
                    _value = value;
                OnChanged?.Invoke(value);
            }
        }

        protected EventList<T> OnChanged { get; set; } = new();

        #region protected WaitAsync

        protected async Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(CancellationToken cancellationToken, params T[] waitValues)
        {
            var taskComplitionSource = new TaskCompletionSource<T>();
            var onChanged = OnChanged.Subscribe(newValue =>
            {
                if (waitValues.Length == 0 || waitValues.Contains(newValue))
                    taskComplitionSource.TrySetResult(newValue);
            });
            try
            {
                var newValue = Value;
                if (waitValues.Contains(newValue))
                    return (AwaitStatus.Ok, Task.FromResult(newValue), newValue);
                return await taskComplitionSource.AsCancellationTask(cancellationToken)
                                .AwaitStatusAsync().ConfigureAwait(false);
            }
            finally
            {
                taskComplitionSource.TrySetCanceled();
                OnChanged.Unsubscribe(onChanged);
            }
        }

        protected async Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(CancellationTokenNode cancellationTokenNode, params T[] waitValues)
        {
            var result = await WaitAsync(cancellationTokenNode.Token, waitValues);
            result.ResultStatus = cancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(result.ResultStatus);
            return result;
        }
        #endregion

        #region public WaitAsync with arguments
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(CancellationToken cancellationToken = default)
            => WaitAsync(cancellationToken, new T[0] { });
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, CancellationToken cancellationToken = default)
            => WaitAsync(cancellationToken, waitValue1);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, CancellationToken cancellationToken = default)
            => WaitAsync(cancellationToken, waitValue1, waitValue2);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, T waitValue3, CancellationToken cancellationToken = default)
            => WaitAsync(cancellationToken, waitValue1, waitValue2, waitValue3);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, T waitValue3, T waitValue4, CancellationToken cancellationToken = default)
            => WaitAsync(cancellationToken, waitValue1, waitValue2, waitValue3, waitValue4);

        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(CancellationTokenNode cancellationTokenNode)
            => WaitAsync(cancellationTokenNode, new T[0] { });
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, CancellationTokenNode cancellationTokenNode)
            => WaitAsync(cancellationTokenNode, waitValue1);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, CancellationTokenNode cancellationTokenNode)
            => WaitAsync(cancellationTokenNode, waitValue1, waitValue2);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, T waitValue3, CancellationTokenNode cancellationTokenNode)
            => WaitAsync(cancellationTokenNode, waitValue1, waitValue2, waitValue3);
        public Task<(AwaitStatus ResultStatus, Task<T> Task, T Result)> WaitAsync(T waitValue1, T waitValue2, T waitValue3, T waitValue4, CancellationTokenNode cancellationTokenNode)
            => WaitAsync(cancellationTokenNode, waitValue1, waitValue2, waitValue3, waitValue4);
        #endregion
    }
}
