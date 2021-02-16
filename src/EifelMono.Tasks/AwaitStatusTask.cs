using System.Collections.Generic;
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
        public TTask Task { get; set; }
    }

    public class AwaitStatusTaskResult<TResult> : AwaitStatusTask<Task<TResult>>
    {
        public AwaitStatusTaskResult() : base()
        {
        }
        public AwaitStatusTaskResult(AwaitStatus awaitStatus, Task<TResult> task, TResult result) : base(awaitStatus, task)
        {
            Result = result;
        }
        public TResult Result { get; set; }
    }

    #region WhenAll

    public class AwaitStatusTaskWhenAll : AwaitStatusTaskBase
    {
        public AwaitStatusTask<Task>[] Tasks { get; set; } = new AwaitStatusTask<Task>[] { };
    }

    #endregion

    #region WhenAny
    public class AwaitStatusTaskWhenAny : AwaitStatusTaskWhenAll
    {
    }
    #endregion

}
