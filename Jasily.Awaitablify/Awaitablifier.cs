using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Jasily.Awaitablify.Internal;
using JetBrains.Annotations;

namespace Jasily.Awaitablify
{
    /// <summary>
    /// Provide API for awaitablify any object.
    /// </summary>
    public sealed class Awaitablifier
    {
        private readonly AwaitableAdapterResolver _resolver = new AwaitableAdapterResolver();

        /// <summary>
        /// Awaitablify a object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NotNull]
        public IBaseAwaitable Awaitablify([CanBeNull] object obj)
        {
            var adapter = this._resolver.Resolve(obj?.GetType());

            if (adapter.IsAwaitable)
            {
                return adapter.CreateAwaitable(obj);
            }
            else
            {
                return new NonAwaitable<object>(obj);
            }
        }

        /// <summary>
        /// Awaitablify a object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NotNull]
        public IBaseAwaitable Awaitablify<T>(T obj)
        {
            var adapter = this._resolver.Resolve(typeof(T));

            if (adapter.IsAwaitable)
            {
                return ((IAwaitableAdapter<T>) adapter).CreateAwaitable(obj);
            }
            else
            {
                return new NonAwaitable<T>(obj);
            }
        }
    }

    public static class AwaitablifierExtensions
    {
        public static async Task<object> UnpackAsync([NotNull] this Awaitablifier awaitablifier, object obj)
        {
            if (awaitablifier == null) throw new ArgumentNullException(nameof(awaitablifier));
            
            var awaitable = awaitablifier.Awaitablify(obj);
            while (awaitable.IsSourceAwaitable)
            {
                var val = await awaitable;
                awaitable = awaitablifier.Awaitablify(val);
            }
            return awaitable.GetAwaiter().GetResult();
        }
    }
}
