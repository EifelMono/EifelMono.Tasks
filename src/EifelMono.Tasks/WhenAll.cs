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

        //public static async Task<AwaitStatusTaskWhenAll<AwaitStatusTask<T1>, AwaitStatusTask<T2>>> AwaitStatusAsync<T1, T2>(T1 task1, T2 task2)
        //    where T1 : Task
        //    where T2 : Task
        //{
        //    await Task.WhenAll(task1, task2);

        //    var item1 = task1.AwaitStatusFromTask();
        //    var item2 = task2.AwaitStatusFromTask();
        //    return new AwaitStatusTaskWhenAll<AwaitStatusTask<T1>, AwaitStatusTask<T2>>
        //    {
        //        Items = new AwaitStatusTask<Task>[]
        //        {
        //            item1,
        //            item2
        //        },
        //        Item1 = item1,
        //        Item2 = item2,
        //    };
        //}
    }
}