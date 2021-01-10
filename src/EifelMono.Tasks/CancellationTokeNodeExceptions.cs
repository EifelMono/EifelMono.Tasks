using System;
using System.Threading;

namespace EifelMono.Tasks
{
    public class OperationRootCanceledException: OperationCanceledException
    {
        public OperationRootCanceledException(): base()
        {
        }

        public OperationRootCanceledException(string message) : base(message)
        {
        }

        public OperationRootCanceledException(string message, CancellationToken cancellactionToken) : base(message, cancellactionToken)
        {
        }

        public OperationRootCanceledException(CancellationToken cancellactionToken) : base(cancellactionToken)
        {
        }
    }
}
