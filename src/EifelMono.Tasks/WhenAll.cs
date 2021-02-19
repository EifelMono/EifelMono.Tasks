using System;
using System.Linq;
using System.Threading.Tasks;

namespace EifelMono.Tasks
{
    public static class WhenAll
    {
        public static async Task<AwaitStatusTaskWhenAll> AwaitStatusAsync(params Task[] tasks)
            => new AwaitStatusTaskWhenAll(
                await Task.WhenAll(tasks).AwaitStatusAsync().ConfigureAwait(false),
                tasks.Select(task => task.AwaitStatusFromTask()).ToArray());

        public static async Task<AwaitStatusTaskWhenAll<W1, W2>> AwaitStatusAsync<W1, W2>(W1 when1, W2 when2)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2>(await AwaitStatusAsync(when1.Task, when2.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2
            };

        public static async Task<AwaitStatusTaskWhenAll<W1, W2, W3>> AwaitStatusAsync<W1, W2, W3>(W1 when1, W2 when2, W3 when3)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2, W3>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3
            };

        public static async Task<AwaitStatusTaskWhenAll<W1, W2, W3, W4>> AwaitStatusAsync<W1, W2, W3, W4>(W1 when1, W2 when2, W3 when3, W4 when4)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            where W4 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2, W3, W4>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task, when4.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3,
                When4 = when4
            };

        public static async Task<AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5>> AwaitStatusAsync<W1, W2, W3, W4, W5>(W1 when1, W2 when2, W3 when3, W4 when4, W5 when5)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            where W4 : AwaitStatusTask
            where W5 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task, when4.Task, when5.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3,
                When4 = when4,
                When5 = when5,
            };

        public static async Task<AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5, W6>> AwaitStatusAsync<W1, W2, W3, W4, W5, W6>(W1 when1, W2 when2, W3 when3, W4 when4, W5 when5, W6 when6)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            where W4 : AwaitStatusTask
            where W5 : AwaitStatusTask
            where W6 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5, W6>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task, when4.Task, when5.Task, when6.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3,
                When4 = when4,
                When5 = when5,
                When6 = when6,
            };

        public static async Task<AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5, W6, W7>> AwaitStatusAsync<W1, W2, W3, W4, W5, W6, W7>(W1 when1, W2 when2, W3 when3, W4 when4, W5 when5, W6 when6, W7 when7)
            where W1 : AwaitStatusTask
            where W2 : AwaitStatusTask
            where W3 : AwaitStatusTask
            where W4 : AwaitStatusTask
            where W5 : AwaitStatusTask
            where W6 : AwaitStatusTask
            where W7 : AwaitStatusTask
            => new AwaitStatusTaskWhenAll<W1, W2, W3, W4, W5, W6, W7>(await AwaitStatusAsync(when1.Task, when2.Task, when3.Task, when4.Task, when5.Task, when6.Task, when7.Task).ConfigureAwait(false))
            {
                When1 = when1,
                When2 = when2,
                When3 = when3,
                When4 = when4,
                When5 = when5,
                When6 = when6,
                When7 = when7,
            };
    }
}