using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Application.Data
{
    public static class ServiceExtensions
    {
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoSection = configuration.GetSection("MongoDb");

            services.AddTransient(x =>
            {
                var client = new MongoClient(mongoSection["ConnectionString"]);

                return client.GetDatabase(mongoSection["Database"]);
            });

            services.AddTransient<IRepository<Message>, MongoDbRepository<Message>>(
                x => new MongoDbRepository<Message>("messages", x.GetRequiredService<IMongoDatabase>()));
        }
    }
}
