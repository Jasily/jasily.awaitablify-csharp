using System;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IAwaitableAdapter
    {
        /// <summary>
        /// whether is adapter is valid or not
        /// </summary>
        bool IsAwaitable { get; }

        IBaseAwaitable CreateAwaitable(object instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <returns></returns>
        bool IsCompleted([NotNull] object instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <returns></returns>
        object GetResult([NotNull] object instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="continuation"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        /// <exception cref="InvalidCastException"></exception>
        void OnCompleted([NotNull] object instance, [NotNull]  Action continuation);
    }

    internal interface ITypedAwaitableAdapter<in TTask> : IAwaitableAdapter
    {
        IBaseAwaitable CreateAwaitable(TTask instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        /// <returns></returns>
        bool IsCompleted([NotNull] TTask instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="continuation"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        void OnCompleted([NotNull] TTask instance, [NotNull]  Action continuation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    internal interface IAwaitableAdapter<in TTask> : ITypedAwaitableAdapter<TTask>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        void GetResult([NotNull] TTask instance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    internal interface IAwaitableAdapter<in TTask, out TResult> : ITypedAwaitableAdapter<TTask>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">throw is not IsAwaitable.</exception>
        /// <returns></returns>
        TResult GetResult([NotNull] TTask instance);
    }
}