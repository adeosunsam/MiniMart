using Microsoft.EntityFrameworkCore;
using MiniMart.Application.Interface;
using MiniMart.Domain.Entities;
using MiniMart.Infrastructure;
using MiniMart.Infrastructure.Context;
using static MiniMart.Application.DTO.ProductDto;

namespace MiniMart.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly MiniMartContext _context;

        public ProductService(MiniMartContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateProductAsync(CreateProductDto requests)
        {
            var response = new ApiResponse();

            try
            {
                // Get Product with the same name.
                var existingProducts = await (from i in _context.Products
                                             where requests.ProductDetails.Select(x => x.Name.ToLower())
                                             .Contains(i.Name.ToLower())
                                             select i.Name).AsNoTracking().ToListAsync();

                // Check if the Product with the same name already exists
                if (existingProducts != null && existingProducts.Any())
                {
                    var existingNames = string.Join(", ", existingProducts);
                    response.Message = $"{existingNames} name(s) already exist";
                    return response;
                }

                // If no existing Product is found, proceed to create the new Products item
                await _context.Products.AddRangeAsync(requests.ProductDetails.Select(productDetail => new Product
                {
                    Name = productDetail.Name,
                    Quantity = productDetail.Quantity,
                    Description = productDetail.Description,
                    CostPrice = productDetail.CostPrice,
                    SellingPrice = productDetail.SellingPrice,
                    MinimumStockAlert = productDetail.MinimumStockAlert
                }));

                await _context.SaveChangesAsync();

                response.Message = "Product item created successfully";
                response.Status = true;

                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse> GetProducts()
        {
            var response = new ApiResponse();

            var query = await (from i in _context.Products
                         where i.IsDeleted == false
                         select new GetProduct
                         {
                             Id = i.Id,
                             Name = i.Name,
                             Quantity = i.Quantity,
                             Description = i.Description,
                             CostPrice = i.CostPrice,
                             SellingPrice = i.SellingPrice,
                             MinimumStockAlert = i.MinimumStockAlert
                         }).AsNoTracking().ToListAsync();

            response.Status = true;
            response.Message = "Products retrieved successfully";
            response.Data = query;

            return response;
        }
    }
}
