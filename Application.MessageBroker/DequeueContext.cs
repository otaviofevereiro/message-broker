using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;

namespace Application.MessageBroker
{
    public class DequeueContext<T> : IDequeueContext<T>
    {

        private readonly IModel channel;
        private readonly ulong deliveryTag;
        private Lazy<T> messageLazy;
        private bool accepted;
        private bool rejected;
        private bool rollbacked;
        private bool disposedValue;

        public DequeueContext(IModel channel, ulong deliveryTag, ReadOnlyMemory<byte> body)
        {
            this.channel = channel;
            this.deliveryTag = deliveryTag;
            messageLazy = new Lazy<T>(JsonSerializer.Deserialize<T>(body.ToArray()));
        }

        public T Message => messageLazy.Value;

        public IDequeueContext<T> Accept()
        {
            channel.BasicAck(deliveryTag, multiple: false);
            accepted = true;

            return this;
        }

        public IDequeueContext<T> Reject()
        {
            channel.BasicReject(deliveryTag, requeue: false);

            rejected = true;

            return this;
        }

        public IDequeueContext<T> Requeue()
        {
            channel.BasicNack(deliveryTag, multiple: false, requeue: true);
            rollbacked = false;

            return this;
        }

        internal void Ensure()
        {
            if (!accepted && !rollbacked && !rejected)
                throw new InvalidOperationException("É necessário ao fim da execução indicar o Commit, Rollback ou Invalidate da mensagem.");
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channel.Dispose();
                }

                basicDeliverEventArgs = null;
                messageLazy = null;
                disposedValue = true;

                Ensure();
            }
        }

        ~DequeueContext()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
