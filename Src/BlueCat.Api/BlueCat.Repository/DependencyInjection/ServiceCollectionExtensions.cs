using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlueCat.Repository
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongodbRepository(this IServiceCollection services)
        {
            // add context
            services.AddSingleton<TelsafeScaffoldMongodbContextSample>();

            // add repository

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddDBConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DBConfig>(configuration.GetSection("DBConfig"));

            return services;
        }
    }
}
