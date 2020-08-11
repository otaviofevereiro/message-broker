using Application.Data;
using Application.MessageBroker;
using Application.Messages;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Consumer.Schedulling
{
    public class MessageConsumerSchedulling : BackgroundService
    {
        private readonly IMessageQueue messageQueue;
        private readonly IRepository<Data.Message> repository;

        public MessageConsumerSchedulling(IMessageQueue messageQueue, IRepository<Data.Message> repository)
        {
            this.messageQueue = messageQueue;
            this.repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            messageQueue.Dequeue<Messages.Message>(Queues.Messages, OnDequeue);

            await Task.CompletedTask;
        }

        private void OnDequeue(IDequeueContext<Messages.Message> dequeueContext)
        {
            repository.Add(new Data.Message()
            {
                Text = dequeueContext.Message.Text,
                To = dequeueContext.Message.To
            });

            dequeueContext.Accept();
        }
    }
}
