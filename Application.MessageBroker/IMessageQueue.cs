using System;

namespace Application.MessageBroker
{
    public interface IMessageQueue : IDisposable
    {
        void CreateMessageReceiver<T>(string queueName, Action<IMessageContext<T>> onReceiveMessageAction);
        IMessageContext<T> Dequeue<T>(string queueName);

        void Enqueue<T>(string queueName, T obj);
    }
}