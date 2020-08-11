using System;

namespace Application.MessageBroker
{
    public interface IDequeueContext<T>: IDisposable
    {
        T Message { get; }

        IDequeueContext<T> Accept();
        IDequeueContext<T> Reject();
        IDequeueContext<T> Requeue();
    }
}