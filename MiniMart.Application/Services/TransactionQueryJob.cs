using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniMart.Application.Interface;

namespace MiniMart.Application.Services
{
    public class TransactionQueryJob : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionQueryJob> _log;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public TransactionQueryJob(IServiceProvider serviceProvider, ILogger<TransactionQueryJob> log)
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
            if (!await _semaphore.WaitAsync(0))
            {
                _log.LogWarning("job is already running. Skipping this run.");
                return;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                await paymentService.TransactionRequery();
            }
            finally
            {
                _semaphore.Release();
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
            _semaphore?.Dispose();
        }
    }
}
