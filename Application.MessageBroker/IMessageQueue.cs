using System;

namespace Application.MessageBroker
{
    public interface IMessageQueue : IDisposable
    {
        void Enqueue<T>(string queueName, T obj);

        void Dequeue<T>(string queueName, Action<IDequeueContext<T>> dequeueAction);
    }
}