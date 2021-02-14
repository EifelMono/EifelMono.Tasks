using System.Collections.Generic;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
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

    public class AwaitStatusTask: AwaitStatusTaskBase
    {
        public AwaitStatusTask(): base()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, Task task): base(awaitStatus)
        {
            Task = task;
        }
        public Task Task { get; set; }
    }

    public class AwaitStatusTaskResult<T> : AwaitStatusTask
    {
        public AwaitStatusTaskResult(): base()
        {
        }
        public AwaitStatusTaskResult(AwaitStatus awaitStatus, Task<T> task, T result): base(awaitStatus, task)
        {
            AwaitStatus = awaitStatus;
            Task = task;
            Result = result;
        }
        public new Task<T> Task { get; set; }
        public T Result { get; set; }
    }

    public class AwaitStatusTaskWhenAll: AwaitStatusTaskBase
    {
        public AwaitStatusTaskWhenAll(): base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatus awaitStatus, List<Task> tasks): base(awaitStatus)
        {
            AwaitStatus = awaitStatus;
            Tasks = tasks;
        }

        public List<Task> Tasks { get; set; } = new List<Task>();
    }

    public class AwaitStatusTaskWhenAny: AwaitStatusTaskWhenAll
    {
        public AwaitStatusTaskWhenAny(): base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatus awaitStatus, List<Task> tasks) : base(awaitStatus, tasks)
        {
        }

        public Task Task { get; set; }
    }
}
