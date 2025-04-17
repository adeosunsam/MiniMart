using Microsoft.Extensions.DependencyInjection;
using MiniMart.Infrastructure.RestSharpHelper;

namespace MiniMart.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void InfrastructureServiceRegistration(this IServiceCollection services)
        {
            services.AddSingleton<IRestSharpClient, RestSharpClient>();
        }
    }
}
