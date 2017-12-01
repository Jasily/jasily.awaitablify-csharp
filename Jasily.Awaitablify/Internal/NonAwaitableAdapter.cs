using System;

namespace Jasily.Awaitablify.Internal
{
    internal sealed class NonAwaitableAdapter : IAwaitableAdapter
    {
        public bool IsAwaitable => false;

        public bool HasResult => false;

        public IBaseAwaitable CreateAwaitable(object instance)
        {
            throw new NotSupportedException();
        }

        public bool IsCompleted(object instance)
        {
            throw new InvalidOperationException("is NOT awaitable.");
        }

        public object GetResult(object instance)
        {
            throw new InvalidOperationException("is NOT awaitable.");
        }

        public void OnCompleted(object instance, Action continuation)
        {
            throw new InvalidOperationException("is NOT awaitable.");
        }

        public static NonAwaitableAdapter Instance { get; } = new NonAwaitableAdapter();
    }
}