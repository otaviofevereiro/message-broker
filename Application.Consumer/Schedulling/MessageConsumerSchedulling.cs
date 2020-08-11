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
            messageQueue.CreateMessageReceiver<Messages.Message>(Queues.Messages, OnMessageReceived);

            await Task.CompletedTask;
        }

        private void OnMessageReceived(IMessageContext<Messages.Message> context)
        {
            repository.Add(new Data.Message()
            {
                Text = context.Message.Text,
                To = context.Message.To
            });

            context.Accept();
        }
    }
}
