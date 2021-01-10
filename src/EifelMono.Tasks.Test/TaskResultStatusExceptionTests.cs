﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test
{
    public class TaskResultStatusExceptionTests
    {
        private static async Task TestTaskAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
            throw new Exception("");
        }

        [Fact]
        public async void Await_Task_throws_an_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            try
            {
                await task;
                Assert.True(false, "This branch is wrong");
            }
            catch (OperationCanceledException)
            {
                Assert.True(false, "This branch is correct");
            }
            catch (Exception)
            {
                Assert.True(true, "This branch is wrong");
            }
        }

        [Fact]
        public async void Await_TaskWhenAny_doesnot_throw_an_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
            Assert.True(task.IsFaulted);
        }

        [Fact]
        public async void Await_Task_with_ResultStatus_and_no_exception()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token)
                .ResultStatusAsync();
            var result = await task;
            Assert.True(result.IsFaulted());
        }

        [Fact]
        public async void Await_TaskWhenAny_with_ResultStatus_and_get_StatusResult()
        {
            using var cts = new CancellationTokenSource();
            var task = TestTaskAsync(cts.Token);
            var resultTask = await Task.WhenAny(task);
            Assert.Equal(task, resultTask);
            Assert.True(task.IsFaulted);
            var result = task.ResultStatus();
            Assert.True(result.IsFaulted());
        }
    }
}
