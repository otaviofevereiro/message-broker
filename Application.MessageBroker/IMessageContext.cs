using System;

namespace Application.MessageBroker
{
    public interface IMessageContext<T>: IDisposable
    {
        T Message { get; }

        IMessageContext<T> Accept();
        IMessageContext<T> Reject();
        IMessageContext<T> Requeue();
    }
}