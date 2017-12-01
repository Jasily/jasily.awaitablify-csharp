using System;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class NonAwaitable<TInstance> : IAwaitable<TInstance>, IAwaiter<TInstance>
    {
        private readonly TInstance _obj;

        public NonAwaitable(TInstance obj)
        {
            this._obj = obj;
        }

        public IAwaiter<TInstance> GetAwaiter() => this;

        IBaseAwaiter IBaseAwaitable.GetAwaiter() => this;

        public bool IsSourceAwaitable => false;

        public void OnCompleted([NotNull] Action continuation)
        {
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));
            continuation();
        }

        public void UnsafeOnCompleted([NotNull] Action continuation)
        {
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));
            continuation();
        }

        public bool IsCompleted => true;

        public bool HasResult => true;

        object IBaseAwaiter.GetResult() => this.GetResult();

        public TInstance GetResult() => this._obj;
    }
}