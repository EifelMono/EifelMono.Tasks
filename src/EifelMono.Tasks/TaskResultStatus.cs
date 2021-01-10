using System;
using System.Linq;

namespace EifelMono.Tasks
{
    [Flags]
    public enum TaskResultStatus
    {
        Unknown = 0x0000,

        Ok = 0x0001,
        Faulted = 0x0002,
        LookAtTaskDotStatusForMore = 0x0004,
        Canceled = 0x0008,

        NodeCanceled = 0x0010,
        RootCanceled = 0x0020,
        BranchCanceled = 0x0040,
        TimeoutCanceled = 0x0080,
        ExternalsCanceled = 0x0100,
    }

    public static class TaskResultStatusExtensions
    {
        public static bool Is(this TaskResultStatus thisValue, params TaskResultStatus[] compareValues)
            => compareValues.Where(value => thisValue == value).Any();

        public static bool Contains(this TaskResultStatus thisValue, params TaskResultStatus[] values)
            => values.Any(value => (thisValue & value) > 0);

        public static TaskResultStatus Include(ref this TaskResultStatus thisValue, params TaskResultStatus[] values)
        {
            foreach (var value in values)
                thisValue |= value;
            return thisValue;
        }

        public static TaskResultStatus Exclude(ref this TaskResultStatus thisValue, params TaskResultStatus[] values)
        {
            foreach (var value in values)
                thisValue &= ~value;
            return thisValue;
        }

        public static bool IsOk(this TaskResultStatus thisValue)
            => thisValue.Is(TaskResultStatus.Ok);
        public static bool IsFaulted(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.Faulted);
        public static bool IsLookAtTaskDotStatusForMore(this TaskResultStatus thisValue)
            => thisValue.Is(TaskResultStatus.LookAtTaskDotStatusForMore);

        public static bool IsCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.Canceled);
        public static bool IsNodeCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.NodeCanceled);
        public static bool IsRootCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.RootCanceled);
        public static bool IsBranchCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.BranchCanceled);
        public static bool IsTimeoutCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.TimeoutCanceled);
        public static bool IsExternalsCanceled(this TaskResultStatus thisValue)
            => thisValue.Contains(TaskResultStatus.ExternalsCanceled);
    }
}
