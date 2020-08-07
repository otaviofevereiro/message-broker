using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.MessageBroker
{
    public class RabbitMQHealthCheck : IHealthCheck
    {
        private readonly IConnectionFactory connectionFactoryMq;

        public RabbitMQHealthCheck(IConnectionFactory connectionFactoryMq)
        {
            this.connectionFactoryMq = connectionFactoryMq;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = connectionFactoryMq.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    return await Task.FromResult(HealthCheckResult.Healthy("RabittMQ is Healthy"));
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy("RabittMQ is Unhealthy", ex));
            }
        }
    }
}
