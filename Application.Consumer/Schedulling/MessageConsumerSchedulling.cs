using Application.Data;
using Application.MessageBroker;
using Application.Messages;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Consumer.Schedulling
{
    public class MessageConsumerSchedulling : IHostedService
    {
        private readonly IMessageQueue messageQueue;
        private readonly IRepository<Data.Message> repository;

        public MessageConsumerSchedulling(IMessageQueue messageQueue, IRepository<Data.Message> repository)
        {
            this.messageQueue = messageQueue;
            this.repository = repository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            messageQueue.Dequeue<Messages.Message>(Queues.Messages,
                context =>
                {
                    repository.Add(new Data.Message()
                    {
                        Text = context.Message.Text,
                        To = context.Message.To
                    });

                    context.Commit();
                }
            );

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(-1, cancellationToken);
        }
    }
}
