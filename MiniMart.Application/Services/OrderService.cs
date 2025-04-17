using Azure;
using Microsoft.EntityFrameworkCore;
using MiniMart.Application.Interface;
using MiniMart.Domain.Entities;
using MiniMart.Infrastructure;
using MiniMart.Infrastructure.Context;
using static MiniMart.Application.DTO.OrderDto;

namespace MiniMart.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly MiniMartContext _context;

        public OrderService(MiniMartContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> PurchaseGoodsAsync(PurchaseProductDto request)
        {
            var response = new ApiResponse();

            try
            {
                // Fetch products matching the requested products
                var products = await (from p in _context.Products
                                      where request.Products.Select(x => x.ProductId).Contains(p.Id)
                                      && !p.IsDeleted
                                      select p).ToDictionaryAsync(x => x.Id);

                // Check if we have matching products for every requested product
                if (products.Count != request.Products.Count)
                {
                    response.Message = "Some products were not found in inventory.";
                    return response;
                }

                //Confirm that each product quantity does not exceed requested quantity
                if (request.Products.Any(x => x.Quantity > products[x.ProductId].Quantity))
                {
                    response.Message = "Product quantity exceeded available quantity";
                    return response;
                }

                var totalPrice = request.Products.Select(x => products[x.ProductId].SellingPrice * x.Quantity).Sum();
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    TotalPrice = totalPrice
                };

                await _context.Orders.AddAsync(order);

                var orderItems = request.Products.Select(x => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList();

                await _context.OrderItems.AddRangeAsync(orderItems);

                foreach (var product in request.Products)
                {
                    products[product.ProductId].Quantity -= product.Quantity;
                }

                await _context.SaveChangesAsync();

                response.Status = true;
                response.Message = "Thanks for shopping with us.";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse> FetchOrderItemsAsync(PurchaseProductDto request)
        {
            var response = new ApiResponse();

            // Fetch products matching the requested products
            var products = await (from p in _context.Products
                                  where request.Products.Select(x => x.ProductId).Contains(p.Id)
                                  && !p.IsDeleted
                                  select p).AsNoTracking().ToDictionaryAsync(x => x.Id);

            // Check if we have matching products for every requested product
            if (products.Count != request.Products.Count)
            {
                response.Message = "Some products were not found in inventory.";
                return response;
            }

            //Confirm that each product quantity does not exceed requested quantity
            if (request.Products.Any(x => x.Quantity > products[x.ProductId].Quantity))
            {
                response.Message = "Product quantity exceeded available quantity";
                return response;
            }

            var orderItems = request.Products.Select(x => new OrderItemDto
            {
                Name = products[x.ProductId].Name,
                Quantity = x.Quantity,
                SellingPrice = products[x.ProductId].SellingPrice,
                TotalItemPrice = x.Quantity * products[x.ProductId].SellingPrice
            }).ToList();

            var totalPrice = request.Products.Select(x => products[x.ProductId].SellingPrice * x.Quantity).Sum();

            var result = new OrderItemResponseDto
            {
                Items = orderItems,
                GrandTotal = totalPrice
            };

            return new ApiResponse
            {
                Status = true,
                Message = "Order summary",
                Data = result
            };
        }
    }
}
