using System.Linq;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    /// <summary>
    /// await Status Task
    /// await Status Task Result
    /// </summary>
    ///

    public class AwaitStatusTaskBase
    {
        public AwaitStatusTaskBase()
        {
        }
        public AwaitStatusTaskBase(AwaitStatus awaitStatus)
        {
            _awaitStatus = awaitStatus;
        }

        protected virtual AwaitStatus GetAwaitStatus()
            => _awaitStatus;

        protected AwaitStatus _awaitStatus { get; set; } = AwaitStatus.Unknown;
        public AwaitStatus AwaitStatus => GetAwaitStatus();
    }

    public class AwaitStatusTask : AwaitStatusTaskBase
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask( Task task) : base(AwaitStatus.Unknown)
        {
            Task = task;
        }
        protected override AwaitStatus GetAwaitStatus()
        {
            var awaitStatus = base.GetAwaitStatus();
            if (awaitStatus == AwaitStatus.Unknown)
                awaitStatus = AwaitStatus.TaskNotAssigned;
            if (Task is { })
            {
                awaitStatus = Task.AwaitStatusOnlyFromTask();
                if (CancellationTokenNode is { })
                    awaitStatus = CancellationTokenNode.AwaitStatusOnlyFromCancellationTokenNode(awaitStatus);
            }
            return awaitStatus;
        }

        public bool IsTaskValid => Task is { };
        public Task Task { get; set; }
        public TaskStatus TaskStatus => Task?.Status ?? TaskStatus.Faulted;

        public CancellationTokenNode CancellationTokenNode { get;protected set; }
        internal void SetCancellationTokenNode(CancellationTokenNode cancellationTokenNode)
           => CancellationTokenNode = cancellationTokenNode;
    }

    public class AwaitStatusTask<TResult> : AwaitStatusTask
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask(Task<TResult> task) : base(task)
        {
        }

        public TResult Result
        {
            get
            {
                if (IsTaskValid)
                    return Task.Result;
                return default;
            }
        }

        public new Task<TResult> Task { get => base.Task as Task<TResult>; set => base.Task = value; }
    }

    public class AwaitStatusTasks : AwaitStatusTaskBase
    {
        public AwaitStatusTask[] Items { get; set; } = new AwaitStatusTask[] { };
        public AwaitStatusTask[] Canceled => Items.Where(ast => ast.IsCanceled()).ToArray();
        public AwaitStatusTask[] Faulted => Items.Where(ast => ast.IsFaulted()).ToArray();
    }
    #region WhenAll

    public class AwaitStatusTaskWhenAll : AwaitStatusTasks
    {
    }


    public class AwaitStatusTaskWhenAll<A1, A2> : AwaitStatusTasks
        where A1 : AwaitStatusTask
        where A2 : AwaitStatusTask
    {
        public A1 Item1 { get; set; }
        public A2 Item2 { get; set; }
    }

    #endregion

    #region WhenAny
    public class AwaitStatusTaskWhenAny : AwaitStatusTasks
    {
    }
    #endregion

}
