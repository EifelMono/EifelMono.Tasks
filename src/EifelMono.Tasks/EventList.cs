using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    #region EventListItem
    internal class EventListItem
    {
        internal bool IsTaskAction { get; set; } = false;
        internal object Action { get; set; } = null;

        internal static EventListItem CreateVoidAction(object voidAction)
            => new() { IsTaskAction = false, Action = voidAction };
        internal static EventListItem CreateTaskAction(object taskAction)
            => new() { IsTaskAction = true, Action = taskAction };
    }
    #endregion

    #region EventList Core
    public abstract class EventListCore: IDisposable
    {
        private readonly object _lockItems = new();
        internal List<EventListItem> Items { get; set; } = new();

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            lock (_lockItems)
            {
                Items.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_lockItems)
                    return Items.Count;
            }
        }

        internal List<EventListItem> Clone()
        {
            lock (_lockItems)
                return Items.Select(item => item).ToList();
        }

        internal EventListItem this[int index]
            => Clone()[index];

        internal object Subscribe(EventListItem item)
        {
            lock (_lockItems)
            {
                Items.Add(item);
            }
            return item;
        }

        internal object Unsubscribe(EventListItem item)
        {
            lock (_lockItems)
            {
                var unsubscribeItem = Items.Find(i => i.Action.Equals(item.Action));
                if (unsubscribeItem is { })
                    Items.Remove(unsubscribeItem);
            }
            return item;
        }

        internal bool Contains(EventListItem item)
        {
            lock (_lockItems)
                return Items.Any(i => i.Action.Equals(item.Action));
        }

        internal void ForEachInvoke(Action<EventListItem> invokeListItemAction, bool parallel = false)
        {
            var clonedItems = Clone();
            void SingleInvoke(EventListItem item)
            {
                try
                {
                    invokeListItemAction?.Invoke(item);
                }
                catch { }
            }
            if (parallel)
                Parallel.ForEach(clonedItems, item => SingleInvoke(item));
            else
                clonedItems.ForEach(item => SingleInvoke(item));
        }
    }
    #endregion

    #region EventList without arguments
    public class EventList : EventListCore
    {
        public Action Subscribe(Action voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<Task> Subscribe(Func<Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action Unsubscribe(Action voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<Task> Unsubscribe(Func<Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(bool parallel = false)
        {
            static async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<Task>)?.Invoke();
                else
                    (invokeListItem.Action as Action)?.Invoke();
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }
    #endregion

    #region EventList with arguments <T1> ... <T1, T2, T3, T4, T5>
    public class EventList<T1> : EventListCore
    {
        public Action<T1> Subscribe(Action<T1> voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }

        public Func<T1, Task> Subscribe(Func<T1, Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action<T1> Unsubscribe(Action<T1> voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }

        public Func<T1, Task> Unsubscribe(Func<T1, Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action<T1> voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<T1, Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(T1 arg1, bool parallel = false)
        {
            async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<T1, Task>)?.Invoke(arg1);
                else
                    (invokeListItem.Action as Action<T1>)?.Invoke(arg1);
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }

    public class EventList<T1, T2> : EventListCore
    {
        public Action<T1, T2> Subscribe(Action<T1, T2> voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, Task> Subscribe(Func<T1, T2, Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action<T1, T2> Unsubscribe(Action<T1, T2> voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, Task> Unsubscribe(Func<T1, T2, Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action<T1, T2> voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<T1, T2, Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(T1 arg1, T2 arg2, bool parallel = false)
        {
            async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<T1, T2, Task>)?.Invoke(arg1, arg2);
                else
                    (invokeListItem.Action as Action<T1, T2>)?.Invoke(arg1, arg2);
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }

    public class EventList<T1, T2, T3> : EventListCore
    {
        public Action<T1, T2, T3> Subscribe(Action<T1, T2, T3> voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, Task> Subscribe(Func<T1, T2, T3, Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action<T1, T2, T3> Unsubscribe(Action<T1, T2, T3> voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, Task> Unsubscribe(Func<T1, T2, T3, Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action<T1, T2, T3> voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<T1, T2, T3, Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, bool parallel = false)
        {
            async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<T1, T2, T3, Task>)?.Invoke(arg1, arg2, arg3);
                else
                    (invokeListItem.Action as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }

    public class EventList<T1, T2, T3, T4> : EventListCore
    {
        public Action<T1, T2, T3, T4> Subscribe(Action<T1, T2, T3, T4> voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, T4, Task> Subscribe(Func<T1, T2, T3, T4, Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action<T1, T2, T3, T4> Unsubscribe(Action<T1, T2, T3, T4> voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, T4, Task> Unsubscribe(Func<T1, T2, T3, T4, Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action<T1, T2, T3, T4> voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<T1, T2, T3, T4, Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool parallel = false)
        {
            async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<T1, T2, T3, T4, Task>)?.Invoke(arg1, arg2, arg3, arg4);
                else
                    (invokeListItem.Action as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }

    public class EventList<T1, T2, T3, T4, T5> : EventListCore
    {
        public Action<T1, T2, T3, T4, T5> Subscribe(Action<T1, T2, T3, T4, T5> voidAction)
        {
            Subscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, T4, T5, Task> Subscribe(Func<T1, T2, T3, T4, T5, Task> taskAction)
        {
            Subscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public Action<T1, T2, T3, T4, T5> Unsubscribe(Action<T1, T2, T3, T4, T5> voidAction)
        {
            Unsubscribe(EventListItem.CreateVoidAction(voidAction));
            return voidAction;
        }
        public Func<T1, T2, T3, T4, T5, Task> Unsubscribe(Func<T1, T2, T3, T4, T5, Task> taskAction)
        {
            Unsubscribe(EventListItem.CreateTaskAction(taskAction));
            return taskAction;
        }

        public bool Contains(Action<T1, T2, T3, T4, T5> voidAction)
            => Contains(EventListItem.CreateVoidAction(voidAction));
        public bool Contains(Func<T1, T2, T3, T4, T5, Task> taskAction)
            => Contains(EventListItem.CreateTaskAction(taskAction));

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool parallel = false)
        {
            async void FireAndForgetInvoke(EventListItem invokeListItem)
            {
                if (invokeListItem.IsTaskAction)
                    await (invokeListItem.Action as Func<T1, T2, T3, T4, T5, Task>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
                else
                    (invokeListItem.Action as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
            ForEachInvoke(invokeListItem => FireAndForgetInvoke(invokeListItem), parallel);
        }
    }
    #endregion
}
