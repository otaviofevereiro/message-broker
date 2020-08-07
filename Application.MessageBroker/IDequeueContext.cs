namespace Application.MessageBroker
{
    public interface IDequeueContext<T>
    {
        T Message { get; }

        IDequeueContext<T> Commit();
        IDequeueContext<T> Rollback();
    }
}