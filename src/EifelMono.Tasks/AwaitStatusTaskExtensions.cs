using System;
namespace EifelMono.Tasks
{
    public static class AwaitStatusTaskExtensions
    {
        public static T AssignCancellationTokenNode<T>(this T thisValue, CancellationTokenNode cancellationTokenNode) where T : AwaitStatusTask
        {
            thisValue.SetCancellationTokenNode(cancellationTokenNode);
            return thisValue;
        }
    }

}
