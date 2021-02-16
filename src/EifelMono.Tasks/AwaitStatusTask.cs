using System.Collections.Generic;
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
            AwaitStatus = awaitStatus;
        }
        public AwaitStatus AwaitStatus { get; set; }
    }

    public class AwaitStatusTask<TTask> : AwaitStatusTaskBase where TTask : Task
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, TTask task) : base(awaitStatus)
        {
            Task = task;
        }
        public bool IsTaskValue => Task is { };
        public TTask Task { get; set; }
    }

    public class AwaitStatusTaskResult<TResult> : AwaitStatusTask<Task<TResult>>
    {
        public AwaitStatusTaskResult() : base()
        {
        }
        public AwaitStatusTaskResult(AwaitStatus awaitStatus, Task<TResult> task) : base(awaitStatus, task)
        {
        }

        public TResult Result
        {
            get
            {
                if (IsTaskValue)
                    return Task.Result;
                return default;
            }
        }
    }

    public class AwaitStatusTasks : AwaitStatusTaskBase
    {
        public AwaitStatusTask<Task>[] Items { get; set; } = new AwaitStatusTask<Task>[] { };
        public AwaitStatusTask<Task>[] Canceled => Items.Where(ast => ast.IsCanceled()).ToArray();
        public AwaitStatusTask<Task>[] Faulted => Items.Where(ast => ast.IsFaulted()).ToArray();
    }
    #region WhenAll

    public class AwaitStatusTaskWhenAll : AwaitStatusTasks
    {
    }

    public class AwaitStatusTaskWhenAll<T1, T2> : AwaitStatusTaskWhenAll
        where T1 : AwaitStatusTaskBase
        where T2 : AwaitStatusTaskBase
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }

    #endregion

    #region WhenAny
    public class AwaitStatusTaskWhenAny : AwaitStatusTasks
    {
    }
    #endregion

}
