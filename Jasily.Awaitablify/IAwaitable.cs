using JetBrains.Annotations;

namespace Jasily.Awaitablify
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAwaitable : IBaseAwaitable
    {
        /// <summary>
        /// Get awaiter from the <see cref="IAwaitable"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        new IAwaiter GetAwaiter();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAwaitable<out T> : IBaseAwaitable
    {
        /// <summary>
        /// Get awaiter from the <see cref="IAwaitable{T}"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        new IAwaiter<T> GetAwaiter();
    }
}