using System;
using System.Linq;

namespace EifelMono.Tasks
{
    [Flags]
    public enum AwaitStatus
    {
        Unknown = 0x0000,
        TaskNotAssigned = 0x0001,
        // 0x0002
        // 0x0004
        // 0x0008

        Ok = 0x0010,
        Faulted = 0x0020,
        LookAtTaskDotStatusForMore = 0x0040,
        Canceled = 0x0080,

        NodeCanceled = 0x0100,
        RootCanceled = 0x0200,
        BranchCanceled = 0x0400,
        TimeoutCanceled = 0x0800,
        ExternalsCanceled = 0x1000,
    }

    public static class AwaitStatusExtensions
    {
        public static bool Is(this AwaitStatus thisValue, params AwaitStatus[] compareValues)
            => compareValues.Where(value => thisValue == value).Any();

        public static bool Contains(this AwaitStatus thisValue, params AwaitStatus[] values)
            => values.Any(value => (thisValue & value) > 0);

        public static AwaitStatus Include(ref this AwaitStatus thisValue, params AwaitStatus[] values)
        {
            foreach (var value in values)
                thisValue |= value;
            return thisValue;
        }

        public static AwaitStatus Exclude(ref this AwaitStatus thisValue, params AwaitStatus[] values)
        {
            foreach (var value in values)
                thisValue &= ~value;
            return thisValue;
        }

        public static bool IsOk(this AwaitStatus thisValue)
            => thisValue.Is(AwaitStatus.Ok);
        public static bool IsFaulted(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.Faulted);
        public static bool IsLookAtTaskDotStatusForMore(this AwaitStatus thisValue)
            => thisValue.Is(AwaitStatus.LookAtTaskDotStatusForMore);

        public static bool IsCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.Canceled);
        public static bool IsNodeCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.NodeCanceled);
        public static bool IsRootCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.RootCanceled);
        public static bool IsBranchCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.BranchCanceled);
        public static bool IsTimeoutCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.TimeoutCanceled);
        public static bool IsExternalsCanceled(this AwaitStatus thisValue)
            => thisValue.Contains(AwaitStatus.ExternalsCanceled);
    }
}
