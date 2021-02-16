using System;
using System.Threading.Tasks;
using Xunit;

namespace EifelMono.Tasks.Test.Properties
{

    public enum EnumA
    {
        A,
        B,
        C
    }

    public class ClassA
    {
        public EnumA EnumA { get; set; }
    }


    public class ClassB<T>: ClassA where T : Task
    {
        public T Task { get; set; }
    }


    public class ClassC<TResult> : ClassB<Task<TResult>>
    {
        public bool IsResultValid => Task is { };
        public TResult Result
        {
            get
            {
                if (IsResultValid)
                    return Task.Result;
                return default;
            }
        }
    }

    public static class X
    {
        public async static Task<ClassB<T>> StateAsync<T>(this T thisValue) where T: Task
        {
            await thisValue;
            return new ClassB<T>
            {
                EnumA = EnumA.B,
                Task = thisValue
            };
        }

        public async static Task<ClassC<TResult>> StateAsync<TResult>(this Task<TResult> thisValue) 
        {
            var x= await thisValue;
            return new ClassC<TResult>
            {
                EnumA = EnumA.C,
                Task = thisValue
            };
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

    }


    public class Test
    {
       
        [Fact]
        public async void TestB()
        {
            var ResultB= await X.StateAsync(X.TaskBAsync());
            Assert.Equal(EnumA.B, ResultB.EnumA);
        }

        [Fact]
        public async void TestC()
        {

            var ResultC= await X.StateAsync(X.TaskCAsync());
            Assert.Equal(EnumA.C, ResultC.EnumA);
            Assert.Equal(1, ResultC.Result);
        }
    }
}
