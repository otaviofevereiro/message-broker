using System;

namespace Application.MessageBroker
{
    public interface IMessageQueue : IDisposable
    {
        void Dequeue<T>(string queueName, Action<IDequeueContext<T>> dequeueAction);
        IDequeueContext<T> Dequeue<T>(string queueName);

        void Enqueue<T>(string queueName, T obj);
    }
}