using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class AwaitableInfoResolver
    {
        private readonly Func<Type, Lazy<AwaitableInfo>> _factory;
        private readonly ConcurrentDictionary<Type, Lazy<AwaitableInfo>> _cache = new ConcurrentDictionary<Type, Lazy<AwaitableInfo>>();

        public AwaitableInfoResolver()
        {
            this._factory = this.CreateLazy;
        }

        [CanBeNull]
        public AwaitableInfo Resolve(Type type)
        {
            return this._cache.GetOrAdd(type, this._factory).Value;
        }

        private Lazy<AwaitableInfo> CreateLazy(Type type)
        {
            return new Lazy<AwaitableInfo>(() => AwaitableInfo.TryCreate(type));
        }
    }
}