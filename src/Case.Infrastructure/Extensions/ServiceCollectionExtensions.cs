using Microsoft.Extensions.DependencyInjection;
using System;

namespace Case.Infrastructure.Extensions
{
    public static class AspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbSettings(this IServiceCollection services, Action<MongoDbSettings> mongoDbSettings)
        {
            services.Configure<MongoDbSettings>(mongoDbSettings);

            return services;
        }
    }
}
