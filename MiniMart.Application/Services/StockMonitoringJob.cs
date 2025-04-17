using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniMart.Infrastructure.Context;

namespace MiniMart.Application.Services
{
    public class StockMonitoringJob : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockMonitoringJob> _log;

        public StockMonitoringJob(IServiceProvider serviceProvider, ILogger<StockMonitoringJob> log)
        {
            _serviceProvider = serviceProvider;
            _log = log;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run every 2 minutes
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MiniMartContext>();

            var lowStock = await (from p in dbContext.Products
                                  where p.Quantity <= p.MinimumStockAlert
                                  && !p.IsDeleted
                                  select p).AsNoTracking().ToListAsync();
            
            if (!lowStock.Any())
            {
                return;
            }

            foreach (var product in lowStock)
            {
                //log low stock
                _log.LogWarning($"LOW STOCK ALERT: Product Name {product.Name}, Current Stock Level {product.Quantity}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
