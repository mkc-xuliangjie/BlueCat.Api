using BlueCat.MySqlRepository.Context;
using BlueCat.MySqlRepository.Impl;
using BlueCat.MySqlRepository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BlueCat.MySqlRepository.DependencyInjection
{
    /// <summary>
    /// Repository注册
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMySqlRepositoryServices(this IServiceCollection services)
        {
            services.AddSingleton<BlueCatScaffoldContextSample>();

            services.AddScoped<ITestRepository, TestRepository>();

            return services;
        }
    }
}
