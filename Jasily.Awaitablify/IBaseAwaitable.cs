using System;
using JetBrains.Annotations;

namespace Jasily.Awaitablify
{
    public interface IBaseAwaitable
    {
        /// <summary>
        /// Get awaiter from the <see cref="IBaseAwaitable"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        IBaseAwaiter GetAwaiter();

        /// <summary>
        /// Whether the source object awaitable.
        /// </summary>
        bool IsSourceAwaitable { get; }
    }

    public static class BaseAwaitableExtensions
    {
        /// <summary>
        /// Cast <see cref="IBaseAwaitable"/> to <see cref="IAwaitable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="awaitable"></param>
        /// <returns></returns>
        public static IAwaitable<T> HasResult<T>([NotNull] this IBaseAwaitable awaitable)
        {
            if (awaitable == null) throw new ArgumentNullException(nameof(awaitable));
            return (IAwaitable<T>) awaitable;
        }
    }
}