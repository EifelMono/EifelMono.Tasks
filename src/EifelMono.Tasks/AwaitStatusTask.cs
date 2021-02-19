using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{

    // -------------------------------------------------------------------------
    // +-- AwaitStatusTaskBase
    // |    AWaitStatus
    // +--+-- AwaitStatusTask
    // |  |     Task
    // |  |     IsTaskValid
    // |  |     CancellationTokenNode
    // |  +-- AwaitStatusTask<TResult>
    // |        TResult Result
    // +--+-- AwaitStatusTasks
    // |  |     Whens
    //
    //
    // -------------------------------------------------------------------------

    #region Single
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
        public AwaitStatusTask(Task task) : base(AwaitStatus.Unknown)
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

        public CancellationTokenNode CancellationTokenNode { get; protected set; }
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
    #endregion

    #region Multi (When)

    public class AwaitStatusTasks : AwaitStatusTask
    {
        public AwaitStatusTasks() : base()
        {
        }
        public AwaitStatusTasks(AwaitStatusTask awaitStatusTask, AwaitStatusTask[] whens) : base(awaitStatusTask.Task)
        {
            Whens = whens;
        }
        public AwaitStatusTasks(AwaitStatusTasks awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll.Task)
        {
            Whens = awaitStatusTaskWhenAll?.Whens ?? new AwaitStatusTask[] { };
        }

        public AwaitStatusTask[] Whens { get; set; } = new AwaitStatusTask[] { };
        public AwaitStatusTask[] Canceled => Whens.Where(ast => ast.IsCanceled()).ToArray();
        public AwaitStatusTask[] Faulted => Whens.Where(ast => ast.IsFaulted()).ToArray();
    }
    #region WhenAll

    public class AwaitStatusTaskWhenAll : AwaitStatusTasks
    {
        public AwaitStatusTaskWhenAll() : base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTask awaitStatusTask, AwaitStatusTask[] whens) : base(awaitStatusTask, whens)
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTaskWhenAll awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll)
        {
        }
    }

    public class AwaitStatusTaskWhenAll<W1, W2> : AwaitStatusTaskWhenAll
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAll() : base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTaskWhenAll awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll)
        {
        }

        public W1 When1 { get; set; }
        public W2 When2 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<W1, W2, W3> : AwaitStatusTaskWhenAll<W1, W2>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAll() : base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTaskWhenAll awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll)
        {
            Whens = awaitStatusTaskWhenAll?.Whens ?? new AwaitStatusTask[] { };
        }

        public W3 When3 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<W1, W2, W3, W4> : AwaitStatusTaskWhenAll<W1, W2, W3>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
        where W4 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAll() : base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTaskWhenAll awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll)
        {
            Whens = awaitStatusTaskWhenAll?.Whens ?? new AwaitStatusTask[] { };
        }

        public W4 When4 { get; set; }
    }

    public class AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5> : AwaitStatusTaskWhenAll<W1, W2, W3, W4>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
        where W4 : AwaitStatusTask
        where W5 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAll() : base()
        {
        }
        public AwaitStatusTaskWhenAll(AwaitStatusTaskWhenAll awaitStatusTaskWhenAll) : base(awaitStatusTaskWhenAll)
        {
            Whens = awaitStatusTaskWhenAll?.Whens ?? new AwaitStatusTask[] { };
        }

        public W5 When5 { get; set; }
    }

    #endregion

    #region WhenAny
    public class AwaitStatusTaskWhenAny : AwaitStatusTasks
    {
        public AwaitStatusTaskWhenAny() : base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTask awaitStatusTask, AwaitStatusTask[] whens) : base(awaitStatusTask, whens)
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTaskWhenAny awaitStatusTaskWhenAny) : base(awaitStatusTaskWhenAny)
        {
        }
    }

    public class AwaitStatusTaskWhenAny<W1, W2> : AwaitStatusTaskWhenAny
      where W1 : AwaitStatusTask
      where W2 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAny() : base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTaskWhenAny awaitStatusTaskWhenAny) : base(awaitStatusTaskWhenAny)
        {
        }

        public W1 When1 { get; set; }
        public W2 When2 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<W1, W2, W3> : AwaitStatusTaskWhenAny<W1, W2>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAny() : base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTaskWhenAny awaitStatusTaskWhenAny) : base(awaitStatusTaskWhenAny)
        {
        }

        public W3 When3 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<W1, W2, W3, W4> : AwaitStatusTaskWhenAny<W1, W2, W3>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
        where W4 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAny() : base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTaskWhenAny awaitStatusTaskWhenAny) : base(awaitStatusTaskWhenAny)
        {
        }

        public W4 When4 { get; set; }
    }

    public class AwaitStatusTaskWhenAny<W1, W2, W3, W4, W5> : AwaitStatusTaskWhenAny<W1, W2, W3, W4>
        where W1 : AwaitStatusTask
        where W2 : AwaitStatusTask
        where W3 : AwaitStatusTask
        where W4 : AwaitStatusTask
        where W5 : AwaitStatusTask
    {
        public AwaitStatusTaskWhenAny() : base()
        {
        }
        public AwaitStatusTaskWhenAny(AwaitStatusTaskWhenAny awaitStatusTaskWhenAny) : base(awaitStatusTaskWhenAny)
        {
        }

        public W5 When5 { get; set; }
    }
    #endregion
    #endregion
}
