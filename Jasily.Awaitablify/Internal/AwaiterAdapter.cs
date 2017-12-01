using System;
using System.Runtime.CompilerServices;
using Jasily.FunctionInvoker;
using Jasily.FunctionInvoker.ArgumentsResolvers;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class AwaiterAdapter
    {
        protected AwaiterInfo AwaiterInfo { get; }

        protected AwaiterAdapter([NotNull] AwaiterInfo info)
        {
            this.AwaiterInfo = info ?? throw new ArgumentNullException(nameof(info));
        }
    }

    internal abstract class AwaiterAdapter<TAwaiter> : AwaiterAdapter
    {
        private readonly IObjectMethodInvoker<TAwaiter, bool> _isCompletedInvoker;

        protected AwaiterAdapter(AwaiterInfo info, FunctionInvokerResolver funcResolver)
            : base(info)
        {
            this._isCompletedInvoker = funcResolver.Resolve(info.IsCompletedMethod)
                .AsObjectMethodInvoker<TAwaiter, bool>(); ;
        }

        public bool IsCompleted(TAwaiter awaiter) => this._isCompletedInvoker.Invoke(awaiter);

        public void OnCompleted(TAwaiter awaiter, Action continuation)
        {
            ((INotifyCompletion)awaiter).OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(TAwaiter awaiter, Action continuation)
        {
            if (this.AwaiterInfo.UnsafeOnCompletedMethod != null)
            {
                ((ICriticalNotifyCompletion)awaiter).UnsafeOnCompleted(continuation);
            }
            else
            {
                this.OnCompleted(awaiter, continuation);
            }
        }
    }
}