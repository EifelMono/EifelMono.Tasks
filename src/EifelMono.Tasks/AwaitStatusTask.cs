using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public class AwaitStatusTask
    {
        public AwaitStatusTask()
        {
        }
        public AwaitStatusTask(AwaitStatus awaitStatus, Task task)
        {
            AwaitStatus = awaitStatus;
            Task = task;
        }
        public AwaitStatus AwaitStatus { get; set; }
        public Task Task { get; set; }
    }

    public class AwaitStatusTaskResult<T> : AwaitStatusTask
    {
        public AwaitStatusTaskResult()
        {
        }
        public AwaitStatusTaskResult(AwaitStatus awaitStatus, Task<T> task, T result)
        {
            AwaitStatus = awaitStatus;
            Task = task;
            Result = result;
        }
        public new Task<T> Task { get; set; }
        public T Result { get; set; }
    }
}
