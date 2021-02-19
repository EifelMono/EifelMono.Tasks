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
        {
            {
                var result = await AwaitStatusAsync();
                return new AwaitStatusTaskWhenAll<W1, W2>(result)
                {
                    When1 = when1,
                    When2 = when2
                };
            }
        }
    }
}