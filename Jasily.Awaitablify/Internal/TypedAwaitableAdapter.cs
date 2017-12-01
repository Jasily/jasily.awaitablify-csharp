using Jasily.FunctionInvoker;
using Jasily.FunctionInvoker.ArgumentsResolvers;
using System;

namespace Jasily.Awaitablify.Internal
{
    internal class TypedAwaitableAdapter<TInstance, TAwaiter, TResult> : AwaitableAdapter<TInstance, TAwaiter>,
        IAwaitableAdapter<TInstance>,
        IAwaitableAdapter<TInstance, TResult>
    {
        private readonly AwaiterAdapterImpl _awaiterAdapter;

        public TypedAwaitableAdapter(AwaitableInfo info, FunctionInvokerResolver funcResolver)
            : base(info, funcResolver)
        {
            this._awaiterAdapter = new AwaiterAdapterImpl(info.AwaiterInfo, funcResolver);
        }

        protected override AwaiterAdapter<TAwaiter> AwaiterAdapter => this._awaiterAdapter;

        public TResult GetResult(TInstance instance) => this._awaiterAdapter.GetResult(this.GetAwaiter(instance));

        void IAwaitableAdapter<TInstance>.GetResult(TInstance instance) => this.GetResult(instance);

        public override object GetResult(object instance) => this.GetResult((TInstance)instance);

        public override void OnCompleted(object instance, Action continuation) => this.OnCompleted((TInstance)instance, continuation);

        public override IBaseAwaitable CreateAwaitable(TInstance instance)
        {
            return new Awaitable<TInstance, TResult>(instance, this);
        }

        public override IBaseAwaitable CreateAwaitable(object instance)
        {
            return this.CreateAwaitable((TInstance)instance);
        }

        private class AwaiterAdapterImpl : AwaiterAdapter<TAwaiter>
        {
            private readonly IObjectMethodInvoker<TAwaiter, TResult> _getResultInvoker;

            public AwaiterAdapterImpl(AwaiterInfo info, FunctionInvokerResolver funcResolver)
                : base(info, funcResolver)
            {
                this._getResultInvoker = funcResolver.Resolve(info.GetResultMethod).AsObjectMethodInvoker<TAwaiter, TResult>();
            }

            public TResult GetResult(TAwaiter awaiter) => this._getResultInvoker.Invoke(awaiter);
        }
    }
}