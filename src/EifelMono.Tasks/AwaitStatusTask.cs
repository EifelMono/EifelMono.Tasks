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

    public class AwaitStatusTask : AwaitStatusTaskBase
    {
        public AwaitStatusTask() : base()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, Task task) : base(awaitStatus)
        {
            Task = task;
        }
        public Task Task { get; set; }
    }

    public class AwaitStatusTaskResult<T> : AwaitStatusTask
    {
        public AwaitStatusTaskResult() : base()
        {
        }
        public AwaitStatusTaskResult(AwaitStatus awaitStatus, Task<T> task, T result) : base(awaitStatus, task)
        {
            Result = result;
        }
        public new Task<T> Task { get; set; }
        public T Result { get; set; }
    }

    #region WhenAll

    public class AwaitStatusTaskWhenAll : AwaitStatusTaskBase
    {
        public AwaitStatusTask[] AwaitStatusTasks { get; set; } = new AwaitStatusTask[] { };
    }

    public class AwaitStatusTaskWhenAll<T1> : AwaitStatusTaskWhenAll
        where T1 : Task
    {
        public T1 Task1 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<T1, T2> : AwaitStatusTaskWhenAll<T1>
        where T1 : Task
        where T2 : Task
    {
        public T2 Task2 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<T1, T2, T3> : AwaitStatusTaskWhenAll<T1, T2>
        where T1 : Task
        where T2 : Task
        where T3 : Task
    {
        public T3 Task3 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<T1, T2, T3, T4> : AwaitStatusTaskWhenAll<T1, T2, T3>
        where T1 : Task
        where T2 : Task
        where T3 : Task
        where T4 : Task
    {
        public T4 Task4 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<T1, T2, T3, T4, T5> : AwaitStatusTaskWhenAll<T1, T2, T3, T4>
      where T1 : Task
      where T2 : Task
      where T3 : Task
      where T4 : Task
      where T5 : Task
    {
        public T5 Task5 { get; set; }
    }
    #endregion

    #region WhenAny
    public class AwaitStatusTaskWhenAny : AwaitStatusTask
    {
        public AwaitStatusTask AwaitStatusTask { get; set; }
        public AwaitStatusTask[] AwaitStatusTasks { get; set; } = new AwaitStatusTask[] { };
    }

    public class AwaitStatusTaskWhenAny<T1> : AwaitStatusTaskWhenAny
        where T1 : Task
    {
        public T1 Task1 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<T1, T2> : AwaitStatusTaskWhenAny<T1>
        where T1 : Task
        where T2 : Task
    {
        public T2 Task2 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<T1, T2, T3> : AwaitStatusTaskWhenAny<T1, T2>
        where T1 : Task
        where T2 : Task
        where T3 : Task
    {
        public T3 Task3 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<T1, T2, T3, T4> : AwaitStatusTaskWhenAny<T1, T2, T3>
        where T1 : Task
        where T2 : Task
        where T3 : Task
        where T4 : Task
    {
        public T4 Task4 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<T1, T2, T3, T4, T5> : AwaitStatusTaskWhenAny<T1, T2, T3, T4>
      where T1 : Task
      where T2 : Task
      where T3 : Task
      where T4 : Task
      where T5 : Task
    {
        public T5 Task5 { get; set; }
    }
    #endregion

}
