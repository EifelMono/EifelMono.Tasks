﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class WhenAny
    {
        public static async Task<AwaitStatusTaskWhenAny> AwaitStatusAsync(params Task[] tasks)
            => new AwaitStatusTaskWhenAny(
                await Task.WhenAny(tasks).AwaitStatusAsync().ConfigureAwait(false),
                tasks.Select(task => task.AwaitStatusFromTask()).ToArray());

        public static async Task<AwaitStatusTaskWhenAny<W1, W2>> AwaitStatusAsync<W1, W2>(W1 when1, W2 when2)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            => new AwaitStatusTaskWhenAny<W1, W2>(await AwaitStatusAsync(when1.Task, when2.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2
            };
        public static async Task<AwaitStatusTaskWhenAny<W1, W2, W3>> AwaitStatusAsync<W1, W2, W3>(W1 when1, W2 when2, W3 when3)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            => new AwaitStatusTaskWhenAny<W1, W2, W3>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3
            };
    }
}