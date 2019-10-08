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
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITestRepository, TestRepository>();

            return services;
        }
    }
}
