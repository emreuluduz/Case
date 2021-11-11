using Case.Application.Interfaces;
using Case.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Case.Application.Extensions
{
    public static class AspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>()
                    .AddScoped<ISliderService, SliderService>()
                    .AddScoped<ICacheService, CacheService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddDistrubutedRedisCache(this IServiceCollection services, string connectionString)
        {
            services.AddStackExchangeRedisCache(x =>
            {
                x.Configuration = connectionString;
            });

            return services;
        }
    }
}
