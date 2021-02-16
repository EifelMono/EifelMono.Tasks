//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace EifelMono.Tasks
//{
//    public static class WhenAny
//    {
//        #region normal
//        public static async Task<AwaitStatusTaskWhenAny> AwaitStatusAsync(params Task[] tasks)
//        {
//            var awaitStatusTask = await Task.WhenAny(tasks).AwaitStatusAsync().ConfigureAwait(false);

//            if (awaitStatusTask is { })
//                return new AwaitStatusTaskWhenAny
//                {
//                    Task = awaitStatusTask.Task,
//                    AwaitStatus = awaitStatusTask.AwaitStatus,
//                    AwaitStatusTasks = tasks.Select(task => task.AwaitStatusFromTask()).ToArray(),
//                };
//            else
//                return new AwaitStatusTaskWhenAny();
//        }


//        public static async Task<AwaitStatusTaskWhenAny<T1, T2>> AwaitStatusAsync<T1, T2>(T1 task1, T2 task2)
//            where T1 : Task
//            where T2 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync().ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask() ?? new (),
//                    task2?.AwaitStatusFromTask() ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3>> AwaitStatusAsync<T1, T2, T3>(T1 task1, T2 task2, T3 task3)
//            where T1 : Task
//            where T2 : Task
//            where T3 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync().ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask() ?? new (),
//                    task2?.AwaitStatusFromTask() ?? new (),
//                    task3?.AwaitStatusFromTask() ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3, T4>> AwaitStatusAsync<T1, T2, T3, T4>(T1 task1, T2 task2, T3 task3, T4 task4)
//             where T1 : Task
//             where T2 : Task
//             where T3 : Task
//             where T4 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync().ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3, T4>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask() ?? new (),
//                    task2?.AwaitStatusFromTask() ?? new (),
//                    task3?.AwaitStatusFromTask() ?? new (),
//                    task4?.AwaitStatusFromTask() ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3,
//                Task4 = task4
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3, T4, T5>> AwaitStatusAsync<T1, T2, T3, T4, T5>(T1 task1, T2 task2, T3 task3, T4 task4, T5 task5)
//             where T1 : Task
//             where T2 : Task
//             where T3 : Task
//             where T4 : Task
//             where T5 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync().ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3, T4, T5>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask() ?? new (),
//                    task2?.AwaitStatusFromTask() ?? new (),
//                    task3?.AwaitStatusFromTask() ?? new (),
//                    task4?.AwaitStatusFromTask() ?? new (),
//                    task5?.AwaitStatusFromTask() ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3,
//                Task4 = task4,
//                Task5 = task5,
//            };
//        }
//        #endregion

//        #region CancellationTokenNode
//        public static async Task<AwaitStatusTaskWhenAny> AwaitStatusAsync(CancellationTokenNode cancellationTokenNode, params Task[] tasks)
//        {
//            var awaitStatusTask = await Task.WhenAny(tasks).AwaitStatusAsync(cancellationTokenNode).ConfigureAwait(false);

//            if (awaitStatusTask is { })
//                return new AwaitStatusTaskWhenAny
//                {
//                    Task = awaitStatusTask.Task,
//                    AwaitStatus = awaitStatusTask.AwaitStatus,
//                    AwaitStatusTasks = tasks.Select(task => task.AwaitStatusFromTask(cancellationTokenNode)).ToArray(),
//                };
//            else
//                return new AwaitStatusTaskWhenAny();
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2>> AwaitStatusAsync<T1, T2>(CancellationTokenNode cancellationTokenNode, T1 task1, T2 task2)
//            where T1 : Task
//            where T2 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync(cancellationTokenNode).ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task2?.AwaitStatusFromTask(cancellationTokenNode) ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3>> AwaitStatusAsync<T1, T2, T3>(CancellationTokenNode cancellationTokenNode, T1 task1, T2 task2, T3 task3)
//            where T1 : Task
//            where T2 : Task
//            where T3 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync(cancellationTokenNode).ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task2?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task3?.AwaitStatusFromTask(cancellationTokenNode) ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3, T4>> AwaitStatusAsync<T1, T2, T3, T4>(CancellationTokenNode cancellationTokenNode, T1 task1, T2 task2, T3 task3, T4 task4)
//             where T1 : Task
//             where T2 : Task
//             where T3 : Task
//             where T4 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync(cancellationTokenNode).ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3, T4>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task2?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task3?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task4?.AwaitStatusFromTask(cancellationTokenNode) ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3,
//                Task4 = task4
//            };
//        }

//        public static async Task<AwaitStatusTaskWhenAny<T1, T2, T3, T4, T5>> AwaitStatusAsync<T1, T2, T3, T4, T5>(CancellationTokenNode cancellationTokenNode, T1 task1, T2 task2, T3 task3, T4 task4, T5 task5)
//             where T1 : Task
//             where T2 : Task
//             where T3 : Task
//             where T4 : Task
//             where T5 : Task
//        {
//            var awaitStatusTask = await Task.WhenAny(task1).AwaitStatusAsync(cancellationTokenNode).ConfigureAwait(false);

//            return new AwaitStatusTaskWhenAny<T1, T2, T3, T4, T5>
//            {
//                Task = awaitStatusTask?.Task,
//                AwaitStatus = awaitStatusTask?.AwaitStatus ?? AwaitStatus.Unknown,
//                AwaitStatusTasks = new AwaitStatusTask[]
//                {
//                    task1?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task2?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task3?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task4?.AwaitStatusFromTask(cancellationTokenNode) ?? new (),
//                    task5?.AwaitStatusFromTask(cancellationTokenNode) ?? new ()
//                },
//                Task1 = task1,
//                Task2 = task2,
//                Task3 = task3,
//                Task4 = task4,
//                Task5 = task5,
//            };
//        }
//        #endregion
//    }
//}