using Application.MessageBroker;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddRabbitHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddCheck<RabbitMQHealthCheck>("rabbitmq_health_check");

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSection = configuration.GetSection("RabbitMq");

            IConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = rabbitMqSection["Host"],
                Port = Convert.ToInt32(rabbitMqSection["Port"])
            };

            services.AddSingleton(connectionFactory)
                    .AddTransient<IMessageQueue, RabbitMessageQueue>();

            return services;
        }

        public static IApplicationBuilder UseRabbitQueues(this IApplicationBuilder applicationBuilder, params string[] queuesNames)
        {

            try
            {
                var connectionFactory = applicationBuilder.ApplicationServices.GetRequiredService<IConnectionFactory>();

                using (var connection = connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    foreach (var queueName in queuesNames)
                    {
                        channel.QueueDeclare(queueName, true, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return applicationBuilder;
        }
    }
}
