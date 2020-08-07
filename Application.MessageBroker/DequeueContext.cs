using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;

namespace Application.MessageBroker
{
    public class DequeueContext<T> : IDequeueContext<T>
    {
        private readonly BasicDeliverEventArgs basicDeliverEventArgs;
        private readonly IModel channel;
        private readonly Lazy<T> messageLazy;
        private bool commited;
        private bool invalidated;
        private bool rollbacked;

        public DequeueContext(IModel channel, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            this.channel = channel;
            this.basicDeliverEventArgs = basicDeliverEventArgs;
            messageLazy = new Lazy<T>(JsonSerializer.Deserialize<T>(basicDeliverEventArgs.Body.ToArray()));
        }

        public T Message => messageLazy.Value;

        public IDequeueContext<T> Commit()
        {
            channel.BasicAck(basicDeliverEventArgs.DeliveryTag, multiple: false);
            commited = true;

            return this;
        }

        public IDequeueContext<T> Invalidate()
        {
            channel.BasicReject(basicDeliverEventArgs.DeliveryTag, requeue: false);
            invalidated = true;

            return this;
        }

        public IDequeueContext<T> Rollback()
        {
            channel.BasicNack(basicDeliverEventArgs.DeliveryTag, multiple: false, requeue: true);
            rollbacked = false;

            return this;
        }

        internal void Ensure()
        {
            if (!commited && !rollbacked && !invalidated)
                throw new InvalidOperationException("É necessário ao fim da execução indicar o Commit, Rollback ou Invalidate da mensagem.");
        }
    }
}
