using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
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
            Debug.WriteLine("Cheguei");
            Debug.WriteLine(((ConnectionFactory)connectionFactoryMq).HostName + ((ConnectionFactory)connectionFactoryMq).Port);

            connection = connectionFactoryMq.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Dequeue<T>(string queueName, Action<IDequeueContext<T>> dequeueAction)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var dequeueContext = new DequeueContext<T>(channel, ea);

                dequeueAction.Invoke(dequeueContext);
                dequeueContext.Ensure();
            };

            channel.BasicConsume(queueName, autoAck: false, consumer);
        }

        public void EnsureQueue(string queueName)
        {
            channel.QueueDeclare(queueName, true, false, false);
        }

        public void Enqueue<T>(string queueName, T obj)
        {
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: channel.CreateBasicProperties(),
                                 body: JsonSerializer.SerializeToUtf8Bytes(obj));
        }

        #region Dispose
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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
