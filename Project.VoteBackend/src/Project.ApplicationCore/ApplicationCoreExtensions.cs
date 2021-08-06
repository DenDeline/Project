using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.ApplicationCore.Interfaces;
using Project.ApplicationCore.Services;

namespace Project.ApplicationCore
{
    public static class ApplicationCoreExtensions
    {
        public static IServiceCollection AddApplicationCore(this IServiceCollection services)
        {
            
            return services;
        }
    }
}
