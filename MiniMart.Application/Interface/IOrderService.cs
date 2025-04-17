using MiniMart.Application.DTO;
using MiniMart.Infrastructure;
using static MiniMart.Application.DTO.OrderDto;

namespace MiniMart.Application.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse> FetchOrderItemsAsync(PurchaseProductDto request);
        Task<ApiResponse> PurchaseGoodsAsync(PurchaseProductDto requests);
    }
}
