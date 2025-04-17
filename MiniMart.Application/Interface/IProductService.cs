using MiniMart.Application.DTO;
using MiniMart.Infrastructure;

namespace MiniMart.Application.Interface
{
    public interface IProductService
    {
        Task<ApiResponse> CreateProductAsync(ProductDto.CreateProductDto request);
        Task<ApiResponse> GetProducts();
    }
}
