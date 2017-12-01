using Jasily.FunctionInvoker;
using Jasily.FunctionInvoker.ArgumentsResolvers;

namespace Jasily.Awaitablify.Internal
{
    /// <summary>
    /// adapter for <see cref="void"/> return value GetResult()
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TAwaiter"></typeparam>
    internal class VoidAwaitableAdapter<TInstance, TAwaiter> : AwaitableAdapter<TInstance, TAwaiter>,
        IAwaitableAdapter<TInstance>, IAwaitableAdapter<TInstance, object>
    {
        private readonly AwaiterAdapterImpl _awaiterAdapter;

        public VoidAwaitableAdapter(AwaitableInfo info, FunctionInvokerResolver funcResolver)
            : base(info, funcResolver)
        {
            this._awaiterAdapter = new AwaiterAdapterImpl(info.AwaiterInfo, funcResolver);
        }

        protected override AwaiterAdapter<TAwaiter> AwaiterAdapter => this._awaiterAdapter;

        public override IBaseAwaitable CreateAwaitable(TInstance instance)
        {
            return new Awaitable<TInstance>(instance, this);
        }

        public override IBaseAwaitable CreateAwaitable(object instance)
        {
            return this.CreateAwaitable((TInstance)instance);
        } 

        public void GetResult(TInstance instance) => this._awaiterAdapter.GetResult(this.GetAwaiter(instance));

        public override object GetResult(object instance)
        {
            this.GetResult((TInstance)instance);
            return null;
        }

        object IAwaitableAdapter<TInstance, object>.GetResult(TInstance instance)
        {
            this.GetResult(instance);
            return null;
        }

        private class AwaiterAdapterImpl : AwaiterAdapter<TAwaiter>
        {
            private readonly IObjectMethodInvoker<TAwaiter> _getResultInvoker;

            public AwaiterAdapterImpl(AwaiterInfo info, FunctionInvokerResolver funcResolver)
                : base(info, funcResolver)
            {
                this._getResultInvoker = funcResolver.Resolve(this.AwaiterInfo.GetResultMethod).AsObjectMethodInvoker<TAwaiter>();
            }

            public void GetResult(TAwaiter awaiter) => this._getResultInvoker.Invoke(awaiter);
        }
    }
}