using System;
using System.Collections.Concurrent;
using System.Linq;
using Jasily.FunctionInvoker.ArgumentsResolvers;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class AwaitableAdapterResolver
    {
        private readonly Func<Type, Lazy<IAwaitableAdapter>> _factory;
        private readonly ConcurrentDictionary<Type, Lazy<IAwaitableAdapter>> _adapters = 
            new ConcurrentDictionary<Type, Lazy<IAwaitableAdapter>>();
        private readonly FunctionInvokerResolver _functionInvokerResolver = new FunctionInvokerResolver();
        private readonly AwaitableInfoResolver _awaitableInfoResolver = new AwaitableInfoResolver();

        public AwaitableAdapterResolver()
        {
            this._factory = this.CreateLazy;
        }

        [NotNull]
        public IAwaitableAdapter Resolve([CanBeNull] Type type)
        {
            if (type == null) return NonAwaitableAdapter.Instance;

            return this._adapters.GetOrAdd(type, this._factory).Value;
        }

        private Lazy<IAwaitableAdapter> CreateLazy(Type type)
        {
            return new Lazy<IAwaitableAdapter>(() =>
            {
                var info = this._awaitableInfoResolver.Resolve(type);
                if (info == null) return NonAwaitableAdapter.Instance;

                var resultType = info.ResultType;
                var closedType = resultType == typeof(void)
                    ? typeof(VoidAwaitableAdapter<,>).MakeGenericType(type, info.AwaiterInfo.TypeInfo)
                    : typeof(TypedAwaitableAdapter<,,>).MakeGenericType(type, info.AwaiterInfo.TypeInfo, resultType);

                return (IAwaitableAdapter)this._functionInvokerResolver.Resolve(closedType.GetConstructors().Single())
                    .Invoke(new object[] { info, this._functionInvokerResolver });
            });
        }
    }
}