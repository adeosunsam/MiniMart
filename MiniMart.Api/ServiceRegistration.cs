using Microsoft.EntityFrameworkCore;
using MiniMart.Infrastructure;
using MiniMart.Infrastructure.Context;
using MiniMart.Application;
using MiniMart.Infrastructure.Helper;
using MiniMart.Application.Services;


namespace MiniMart.Api
{
    public static class ServiceRegistration
    {
        public static WebApplicationBuilder RegisterCustomServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddDbContext<MiniMartContext>(options =>
            {
                var connStr = configuration["ConnectionStrings:DefaultConnection"];

                options.UseSqlServer(connStr);
            });

            builder.Services.Configure<AppKeys>(configuration.GetSection("AppKeys"));

            builder.Services.ApplicationServiceRegistration();
            builder.Services.InfrastructureServiceRegistration();
            
            builder.Services.AddHostedService<StockMonitoringJob>();
            builder.Services.AddHostedService<TransactionQueryJob>();

            return builder;
        }
    }
}
