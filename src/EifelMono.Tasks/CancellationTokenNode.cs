using System;
using System.Linq;
using System.Threading;

namespace EifelMono.Tasks
{
    // -------------------------------------------------------------------------
    // +- Root
    // |
    // |
    // |
    // | +--- Branch
    // | |+-- BranchTimeout
    // | ||+- BranchExternals
    // | |||
    // | |||
    // +-+++
    //   |
    // Node

    sealed public class CancellationTokenNode : IDisposable
    {
        #region constructor / destructor
        public CancellationTokenNode()
        {
            Root = new();
            Branch = new();
            BranchTimeout = new();
            BranchExternals = new();
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
        }

        public CancellationTokenNode(CancellationTokenSource rootSource)
        {
            Root = new(rootSource);
            Branch = new();
            BranchTimeout = new();
            BranchExternals = new();
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
        }

        public CancellationTokenNode(CancellationToken rootToken)
        {
            Root = new(rootToken);
            Branch = new();
            BranchTimeout = new();
            BranchExternals = new();
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
        }

        public CancellationTokenNode(CancellationTokenNode cancellationTokenNode)
        {
            Root = new(cancellationTokenNode.Node.Source);
            Branch = new();
            BranchTimeout = new();
            BranchExternals = new();
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
        }

        public void Dispose()
        {
            Root?.Dispose();
            Root = null;
            Branch?.Dispose();
            Branch = null;
            BranchTimeout?.Dispose();
            BranchTimeout = null;
            BranchExternals?.Dispose();
            BranchExternals = null;

            Node?.Dispose();
            Node = null;
        }
        #endregion

        #region with....
        public CancellationTokenNode WithTimeout(TimeSpan timeout)
        {
            Node?.Dispose();
            BranchTimeout?.Dispose();
            BranchTimeout = new(timeout);
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
            return this;
        }

        public CancellationTokenNode WithExternals(params CancellationToken[] externalCancellationTokens)
        {
            Node?.Dispose();
            BranchExternals?.Dispose();
            BranchExternals = new(externalCancellationTokens);
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
            return this;
        }

        public CancellationTokenNode WithExternals(params CancellationTokenSource[] externalCancellationTokenSources)
        {
            Node?.Dispose();
            BranchExternals?.Dispose();
            BranchExternals = new(externalCancellationTokenSources.Select(cts=> cts.Token).ToArray());
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
            return this;
        }

        public CancellationTokenNode WithExternals(params CancellationTokenNode[] externalCancellationTokenNode)
        {
            Node?.Dispose();
            BranchExternals?.Dispose();
            BranchExternals = new(externalCancellationTokenNode.Select(ctn => ctn.Token).ToArray());
            Node = new(Root, Branch, BranchTimeout, BranchExternals);
            return this;
        }
        #endregion

        #region Tree
        public CancellationTokenNodeItem Root { get; private set; }
        public CancellationTokenNodeItem Branch { get; private set; }
        public CancellationTokenNodeItem BranchTimeout { get; private set; }
        public CancellationTokenNodeItem BranchExternals { get; private set; }

        public CancellationTokenNodeItem Node { get; private set; }

        public CancellationToken Token => Node.Token;

        public void Reset()
        {
            Root?.Reset();
            Branch?.Reset();
            BranchTimeout?.Reset();
            BranchExternals?.Reset();

            Node?.CancellationTokens.Clear();
            Node?.CancellationTokens.Add(Root.Token);
            Node?.CancellationTokens.Add(Branch.Token);
            Node?.CancellationTokens.Add(BranchTimeout.Token);
            Node?.CancellationTokens.Add(BranchExternals.Token);
            Node?.Reset();
        }
        #endregion

        public bool IsRootCanceled
            => Root.IsCanceled;
    }
}
