using System;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class Awaitable<TInstance> : IAwaitable
    {
        private readonly TInstance _obj;
        private readonly IAwaitableAdapter<TInstance> _adapter;
        private readonly Awaiter _awaiter;

        public Awaitable(TInstance obj, IAwaitableAdapter<TInstance> adapter)
        {
            this._obj = obj;
            this._adapter = adapter;
            this._awaiter = new Awaiter(this);
        }

        public IAwaiter GetAwaiter() => this._awaiter;

        IBaseAwaiter IBaseAwaitable.GetAwaiter() => this.GetAwaiter();

        public bool IsSourceAwaitable => true;

        private class Awaiter : IAwaiter
        {
            private readonly Awaitable<TInstance> _awaitable;

            public Awaiter(Awaitable<TInstance> awaitable)
            {
                this._awaitable = awaitable;
            }

            public void OnCompleted(Action continuation)
            {
                this._awaitable._adapter.OnCompleted(this._awaitable._obj, continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                this._awaitable._adapter.OnCompleted(this._awaitable._obj, continuation);
            }

            public bool IsCompleted => this._awaitable._adapter.IsCompleted(this._awaitable._obj);

            public bool HasResult => false;

            public void GetResult() => this._awaitable._adapter.GetResult(this._awaitable._obj);

            object IBaseAwaiter.GetResult() => null;
        }
    }

    internal class Awaitable<TInstance, TResult> : IAwaitable<TResult>
    {
        private readonly TInstance _obj;
        private readonly IAwaitableAdapter<TInstance, TResult> _adapter;
        private readonly Awaiter _awaiter;

        public Awaitable(TInstance obj, IAwaitableAdapter<TInstance, TResult> adapter)
        {
            this._obj = obj;
            this._adapter = adapter;
            this._awaiter = new Awaiter(this);
        }

        public IAwaiter<TResult> GetAwaiter() => this._awaiter;

        public bool IsSourceAwaitable => true;

        private class Awaiter : IAwaiter<TResult>
        {
            private readonly Awaitable<TInstance, TResult> _awaitable;

            public Awaiter(Awaitable<TInstance, TResult> awaitable)
            {
                this._awaitable = awaitable;
            }

            public void OnCompleted(Action continuation)
            {
                this._awaitable._adapter.OnCompleted(this._awaitable._obj, continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                this._awaitable._adapter.OnCompleted(this._awaitable._obj, continuation);
            }

            public bool IsCompleted => this._awaitable._adapter.IsCompleted(this._awaitable._obj);

            public bool HasResult => true;

            object IBaseAwaiter.GetResult() => this.GetResult();

            public TResult GetResult() => this._awaitable._adapter.GetResult(this._awaitable._obj);
        }

        IBaseAwaiter IBaseAwaitable.GetAwaiter() => this.GetAwaiter();
    }
}