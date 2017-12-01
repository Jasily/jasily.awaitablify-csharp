namespace Jasily.Awaitablify
{
    public interface IAwaiter : IBaseAwaiter
    {
        new void GetResult();
    }

    public interface IAwaiter<out T> : IBaseAwaiter
    {
        new T GetResult();
    }
}