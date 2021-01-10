using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EifelMono.Tasks
{
    public class CancellationTokenNodeItem : IDisposable
    {
        public CancellationTokenNodeItem()
        {
            IsSourceDisposeable = true;
            Source = new CancellationTokenSource();
            Token = Source.Token;
        }

        public CancellationTokenNodeItem(CancellationToken cancellationToken)
        {
            Token = cancellationToken;
        }

        public CancellationTokenNodeItem(CancellationTokenSource cancellationTokenSource)
        {
            Source = cancellationTokenSource;
            Token = Source.Token;
        }

        public CancellationTokenNodeItem(TimeSpan timeout)
        {
            IsSourceDisposeable = true;
            Source = new CancellationTokenSource();
            Token = Source.Token;
            Timeout = timeout;
            if (HasTimeout)
                Source.CancelAfter(timeout);
        }

        public CancellationTokenNodeItem(params CancellationToken[] cancellationTokens)
        {
            IsSourceDisposeable = true;
            if (cancellationTokens is { })
            {
                CancellationTokens.AddRange(cancellationTokens);
                Source = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokens);
            }
            else
            {
                Source = new();
            }
            Token = Source.Token;
        }

        public CancellationTokenNodeItem(params CancellationTokenNodeItem[] cancellationTokenNodeSources)
            : this(cancellationTokenNodeSources.Select(ctns => ctns.Token).ToArray())
        {
        }

        public void Dispose()
        {
            if (IsSourceDisposeable)
                Source?.Dispose();
            Source = null;
            CancellationTokens.Clear();
        }

        public void Reset()
        {
            if (IsSourceDisposeable)
            {
                Source?.Dispose();
                if (CancellationTokens.Count > 0)
                    Source = CancellationTokenSource.CreateLinkedTokenSource(CancellationTokens.ToArray());
                else
                {
                    Source = new CancellationTokenSource();
                    if (HasTimeout)
                        Source.CancelAfter(Timeout);
                }
                Token = Source.Token;
            }
        }

        public CancellationTokenSource Source { get; private set; } = default;
        public CancellationToken Token { get; private set; } = default;
        public TimeSpan Timeout { get; private set; } = default;

        internal List<CancellationToken> CancellationTokens { get; set; } = new List<CancellationToken>();

        public bool IsSourceDisposeable { get; private set; }
        public bool HasSource
            => Source is { };
        public bool HasToken
            => Token != default;
        public bool HasTimeout
            => Timeout != default;

        public void Cancel()
            => Source?.Cancel();
        public bool IsTimeout
            => HasTimeout && Token.IsCancellationRequested;

        public bool IsCancellationRequested
            => Token.IsCancellationRequested;
        public bool IsCanceled
            => IsCancellationRequested;
    }
}
