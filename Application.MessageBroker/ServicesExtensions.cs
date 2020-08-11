using Application.MessageBroker;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;

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
    }
}
