using RabbitMQ.Client;
using System;
using System.Text.Json;

namespace Application.MessageBroker
{
    public class MessageContext<T> : IMessageContext<T>
    {
        private readonly IModel channel;
        private readonly ulong deliveryTag;
        private bool accepted;
        private bool disposedValue;
        private Lazy<T> messageLazy;
        private bool rejected;
        private bool rollbacked;

        public MessageContext(IModel channel, ulong deliveryTag, ReadOnlyMemory<byte> body)
        {
            this.channel = channel;
            this.deliveryTag = deliveryTag;
            messageLazy = new Lazy<T>(() => JsonSerializer.Deserialize<T>(body.ToArray()));
        }

        public T Message => messageLazy.Value;

        public IMessageContext<T> Accept()
        {
            channel.BasicAck(deliveryTag, multiple: false);
            accepted = true;

            return this;
        }

        public IMessageContext<T> Reject()
        {
            channel.BasicReject(deliveryTag, requeue: false);

            rejected = true;

            return this;
        }

        public IMessageContext<T> Requeue()
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
        ~MessageContext()
        {
            Dispose(disposing: false);
        }

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
                    //.Dispose()
                }

                messageLazy = null;
                disposedValue = true;

                Ensure();
            }
        }
        #endregion
    }
}
