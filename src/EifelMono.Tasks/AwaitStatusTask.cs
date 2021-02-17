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

    public class AwaitStatusTask : AwaitStatusTaskBase
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, Task task) : base(awaitStatus)
        {
            Task = task;
        }
        public bool IsTaskValid => Task is { };
        public Task Task { get; set; }
    }

    public class AwaitStatusTask<TResult> : AwaitStatusTask
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, Task<TResult> task) : base(awaitStatus, task)
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
