using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniMart.Application.Interface;
using MiniMart.Application.Services;
using MiniMart.Infrastructure.Context;
using MiniMart.Infrastructure.Helper;
using MiniMart.Infrastructure.RestSharpHelper;
using static MiniMart.Application.DTO.BankLinkDto;
using static MiniMart.Application.DTO.OrderDto;
using static MiniMart.Application.DTO.ProductDto;

namespace MiniMart.ConsoleApp
{
    internal class MiniMartConsole
    {
        private readonly static IServiceProvider serviceProvider;

        public static Dictionary<int, Guid> ProductMapping = [];

        static MiniMartConsole()
        {
            var services = new ServiceCollection();

            services.AddLogging(config =>
            {
                config.ClearProviders();
            });

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddSingleton<IBankLinkService, BankLinkService>();
            services.AddSingleton<IRestSharpClient, RestSharpClient>();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.AddDbContext<MiniMartContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));

            services.Configure<AppKeys>(configuration.GetSection("AppKeys"));

            serviceProvider = services.BuildServiceProvider();
        }

        public static async Task GetProduct()
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                ProductMapping = new Dictionary<int, Guid>();

                var products = await productService.GetProducts();

                var productDetails = products.Data as List<GetProduct>;
                TabularData.PrintLines();
                TabularData.PrintHeadings("S/N", "PRODUCT ID", "PRODUCT NAME", "SELLING PRICE", "QUANTITY LEFT");
                TabularData.PrintLines();
                int i = 0;
                foreach (var item in productDetails)
                {
                    ProductMapping.Add(++i, item.Id);
                    TabularData.PrintHeadings(i.ToString(), item.Id.ToString(), item.Name, item.SellingPrice.ToString(), item.Quantity.ToString());
                    TabularData.PrintLines();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<decimal> DisplayOrder(PurchaseProductDto request)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                var order = await orderService.FetchOrderItemsAsync(request);

                var orderDetails = order.Data as OrderItemResponseDto;
                TabularData.PrintLines();
                TabularData.PrintHeadings("S/N", "PRODUCT NAME", "QUANTITY", "SELLING PRICE", "TOTAL ITEM PRICE");
                TabularData.PrintLines();
                int i = 0;
                foreach (var item in orderDetails.Items)
                {
                    TabularData.PrintHeadings($"{++i}", item.Name, item.Quantity.ToString(), item.SellingPrice.ToString(), item.TotalItemPrice.ToString());
                    TabularData.PrintLines();
                }
                TabularData.PrintHeadings("", "", "", "GRAND TOTAL", $"{orderDetails.GrandTotal}");
                TabularData.PrintLines();

                return orderDetails.GrandTotal;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task PurchaseOrder(PurchaseProductDto request)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                var order = await orderService.PurchaseGoodsAsync(request);

                TabularData.PrintLines();
                TabularData.PrintHeadings($"{order.Message}");
                TabularData.PrintLines();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<(string? traceId, string? errorMessage)> GenerateAccountNumber(GenerateAccountNumberRequestDto request)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                var response = await paymentService.GenerateVirtualAccount(request);

                if (response == null)
                {
                    return (null, response?.Message ?? "error generating account number");
                }

                var accountDetail = response.Data as InvokePaymentResponseDto;

                if (accountDetail == null || accountDetail.ResponseHeader.ResponseCode != ResponseCode.Successful)
                {
                    return (null, accountDetail?.ResponseHeader?.ResponseMessage ?? "error generating account number");
                }

                TabularData.PrintLines();
                TabularData.PrintHeadings("ACCOUNT NUMBER", "ACCOUNT NAME", "AMOUNT");
                TabularData.PrintLines();

                TabularData.PrintHeadings(accountDetail.DestinationAccountNumber, accountDetail.DestinationAccountName, request.Amount.ToString());
                TabularData.PrintLines();

                return (accountDetail.ResponseHeader.TraceId, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task PaymentConfirmation(string paymentRef)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                var response = await paymentService.PaymentConfirmation(paymentRef);

                TabularData.PrintLines();
                TabularData.PrintHeadings($"{response.Message}");
                TabularData.PrintLines();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
