using Microsoft.Extensions.DependencyInjection;
using MiniMart.Application.Interface;
using MiniMart.Application.Services;

namespace MiniMart.Application
{
    public static class ServiceRegistration
    {
        public static void ApplicationServiceRegistration(this IServiceCollection services)
        {
            //register service alphabetically A-Z
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<IBankLinkService, BankLinkService>();
            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
