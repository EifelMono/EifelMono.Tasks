using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EifelMono.Tasks.Test
{
    public class TestPlay
    {

        private ITestOutputHelper Output { get; }

        public TestPlay(ITestOutputHelper output)
        {
            Output = output;
        }
        [Fact]
        public async void On_Cancel_Task_Test()
        {
            var awaitStatusResult = (await Task.Run(async () =>
            {
                await Task.Delay(1);

            }).AwaitStatusAsync().ConfigureAwait(false))
            .OnOk(s =>
            {
                
                Output.WriteLine("Ok");
            });
        }

        [Fact]
        public async void On_Cancel_TaskInt_Test()
        {
            var awaitStatusResult = (await Task.Run(async () =>
            {
                await Task.Delay(1);
                return 1;

            }).AwaitStatusAsync().ConfigureAwait(false))
            .OnOk(s =>
            {
                Output.WriteLine($"Ok result={s.Result}");
            });
        }
    }
}
