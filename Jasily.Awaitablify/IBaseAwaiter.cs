using System.Runtime.CompilerServices;

namespace Jasily.Awaitablify
{
    public interface IBaseAwaiter : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        bool HasResult { get; }

        object GetResult();
    }
}