using System;
using System.Reflection;
using Jasily.FunctionInvoker;
using JetBrains.Annotations;
using Jasily.FunctionInvoker.ArgumentsResolvers;

namespace Jasily.Awaitablify.Internal
{
    internal abstract class AwaitableAdapter : IAwaitableAdapter
    {
        private readonly bool _isValueType;

        protected AwaitableAdapter(AwaitableInfo info)
        {
            this.AwaitableInfo = info ?? throw new ArgumentNullException(nameof(info));
            this._isValueType = info.AwaitableType.GetTypeInfo().IsValueType;
        }

        protected AwaitableInfo AwaitableInfo { get; }

        public bool IsAwaitable => true;

        public abstract bool IsCompleted(object instance);

        public abstract object GetResult(object instance);

        public abstract void OnCompleted(object instance, Action continuation);

        protected void VerifyArgument<T>([NotNull] T instance)
        {
            if (!this._isValueType && instance == null) throw new ArgumentNullException(nameof(instance));
            if (!this.IsAwaitable) throw new InvalidOperationException("is NOT awaitable.");
        }

        public abstract IBaseAwaitable CreateAwaitable(object instance);
    }

    internal abstract class AwaitableAdapter<TInstance, TAwaiter> : AwaitableAdapter, ITypedAwaitableAdapter<TInstance>
    {
        private readonly IObjectMethodInvoker<TInstance, TAwaiter> _getAwaiterInvoker;

        protected AwaitableAdapter(AwaitableInfo info, FunctionInvokerResolver funcResolver)
            : base(info)
        {
            this._getAwaiterInvoker = funcResolver.Resolve(this.AwaitableInfo.GetAwaiterMethod).AsObjectMethodInvoker<TInstance, TAwaiter>();
        }

        protected abstract AwaiterAdapter<TAwaiter> AwaiterAdapter { get; }

        protected TAwaiter GetAwaiter(TInstance instance)
        {
            this.VerifyArgument(instance);
            return this._getAwaiterInvoker.Invoke(instance);
        }

        public bool IsCompleted(TInstance instance) => this.AwaiterAdapter.IsCompleted(this.GetAwaiter(instance));

        public void OnCompleted(TInstance instance, Action continuation)
        {
            this.AwaiterAdapter.OnCompleted(this.GetAwaiter(instance), continuation);
        }

        public override bool IsCompleted(object instance) => this.IsCompleted((TInstance)instance);

        public override void OnCompleted(object instance, Action continuation) => this.OnCompleted((TInstance)instance, continuation);

        public abstract IBaseAwaitable CreateAwaitable(TInstance instance);
    }
}