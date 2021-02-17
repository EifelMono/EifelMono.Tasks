using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class WhenAll
    {
        public static async Task<AwaitStatusTaskWhenAll> AwaitStatusAsync(params Task[] tasks)
        {
            var awaitStatusTask = await Task.WhenAll(tasks).AwaitStatusAsync().ConfigureAwait(false);

            if (awaitStatusTask is { })
                return new AwaitStatusTaskWhenAll
                {
                    AwaitStatus = awaitStatusTask.AwaitStatus,
                    Items = tasks.Select(task => task.AwaitStatusFromTask()).ToArray(),
                };
            else
                return new AwaitStatusTaskWhenAll();
        }

        public static async Task<AwaitStatusTaskWhenAll> AwaitStatusAsync<A1, A2>(A1 awaitStatusTask1, A2 awaitStatusTask2, Action<A1, A2> action)
            where A1 : AwaitStatusTask
            where A2 : AwaitStatusTask
        {
            {
                var result = await AwaitStatusAsync();
                action(awaitStatusTask1, awaitStatusTask2);
                return result;
            }
        }

        public static async Task TaskBAsync()
        {
            await Task.Delay(1);
        }

        public static async Task<int> TaskCAsync()
        {
            await Task.Delay(1);
            return 1;
        }


        public static async Task TestAsync()
        {
            var resultb = await AwaitStatusAsync(TaskBAsync().AwaitStatusFromTask(), TaskCAsync().AwaitStatusFromTask(), (b, c) =>
             {
                 _ = b.Task;
                 _ = c.Result;
             });

        }

        //public static async Task<(AwaitStatusTask<Task>, AwaitStatusTask<Task>)> AwaitStatusAsync(Task task1, Task task2)
        //{
        //    var result= await AwaitStatusAsync(task1, task2);

        //    var item1 = task1.AwaitStatusFromTask();
        //    var item2 = task2.AwaitStatusFromTask();
        //    return (item1, item2);
        //}

        //public static async Task<(AwaitStatusTask<Task<TResult1>>, AwaitStatusTask<Task<TResult2>>)> AwaitStatusAsync<TResult1, TResult2>(Task<TResult1> task1, Task<TResult2> task2)
        //{
        //    var result = await AwaitStatusAsync(task1, task2);

        //    var item1 = task1.AwaitStatusFromTask();
        //    var item2 = task2.AwaitStatusFromTask();
        //    return (item1, item2);
        //}
    }
}