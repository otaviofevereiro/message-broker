using System;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.MessageBroker
{
    public class RabbitMessageQueue : IMessageQueue
    {
        private readonly IModel channel;
        private readonly IConnection connection;
        private bool disposedValue;

        public RabbitMessageQueue(IConnectionFactory connectionFactoryMq)
        {
            connection = connectionFactoryMq.CreateConnection();
            channel = connection.CreateModel();
        }

        public void CreateMessageReceiver<T>(string queueName, Action<IMessageContext<T>> onReceiveMessageAction)
        {
            EnsureQueue(queueName);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, basicDeliverEventArgs) =>
            {
                using (var dequeueContext = new MessageContext<T>(channel, basicDeliverEventArgs.DeliveryTag, basicDeliverEventArgs.Body))
                {
                    onReceiveMessageAction.Invoke(dequeueContext);
                }
            };

            channel.BasicConsume(queueName, autoAck: false, consumer);
        }

        public IMessageContext<T> Dequeue<T>(string queueName)
        {
            EnsureQueue(queueName);

            var response = channel.BasicGet(queueName, autoAck: false);

            return new MessageContext<T>(channel, response.DeliveryTag, response.Body);
        }

        public void Enqueue<T>(string queueName, T obj)
        {
            EnsureQueue(queueName);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: channel.CreateBasicProperties(),
                                 body: JsonSerializer.SerializeToUtf8Bytes(obj));
        }

        private void EnsureQueue(string queueName)
        {
            channel.QueueDeclare(queueName, true, false, false);
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channel.Dispose();
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Queue()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        #endregion
    }
}
