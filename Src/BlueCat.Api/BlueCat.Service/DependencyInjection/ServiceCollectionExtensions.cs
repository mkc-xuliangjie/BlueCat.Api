using BlueCat.Service.Impl;
using BlueCat.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BlueCat.Service.DependencyInjection
{
    /// <summary>
    /// Services注册
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITestServices, TestService>();

            return services;
        }
    }
}
