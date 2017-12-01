using System;
using System.Collections.Concurrent;
using System.Reflection;
using Jasily.FunctionInvoker;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class FunctionInvokerResolver
    {
        private readonly Func<MethodInfo, Lazy<IMethodInvoker>> _methodFactory;
        private readonly Func<ConstructorInfo, Lazy<IConstructorInvoker>> _constructorFactory;
        private readonly ConcurrentDictionary<MethodInfo, Lazy<IMethodInvoker>> _methodInvokers
            = new ConcurrentDictionary<MethodInfo, Lazy<IMethodInvoker>>();
        private readonly ConcurrentDictionary<ConstructorInfo, Lazy<IConstructorInvoker>> _constructorInvokers
            = new ConcurrentDictionary<ConstructorInfo, Lazy<IConstructorInvoker>>();

        public FunctionInvokerResolver()
        {
            this._methodFactory = this.MethodFactory;
            this._constructorFactory = this.ConstructorFactory;
        }

        public IMethodInvoker Resolve([NotNull] MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return this._methodInvokers.GetOrAdd(method, this._methodFactory).Value;
        }

        public IConstructorInvoker Resolve([NotNull] ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            return this._constructorInvokers.GetOrAdd(constructor, this._constructorFactory).Value;
        }

        private Lazy<IMethodInvoker> MethodFactory([NotNull] MethodInfo method)
        {
            return new Lazy<IMethodInvoker>(() => FunctionInvoker.FunctionInvoker.CreateInvoker(method));
        }

        private Lazy<IConstructorInvoker> ConstructorFactory([NotNull] ConstructorInfo constructor)
        {
            return new Lazy<IConstructorInvoker>(() => FunctionInvoker.FunctionInvoker.CreateInvoker(constructor));
        }
    }
}