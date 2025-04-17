using System.ComponentModel.DataAnnotations;
using MiniMart.Infrastructure.Attributes;

namespace MiniMart.Application.DTO
{
    public class ProductDto
    {
        
        public class CreateProductDto
        {
            public List<ProductDetails> ProductDetails { get; set; }
        }

        public class ProductDetails
        {
            [Required]
            public string Name { get; set; }

            public string? Description { get; set; }
            [GreaterThanZero]
            public decimal CostPrice { get; set; }
            [GreaterThanZero]
            public decimal SellingPrice { get; set; }

            [GreaterThanZero]
            public int Quantity { get; set; }
            [GreaterThanZero]
            public int MinimumStockAlert { get; set; }
        }

        public class GetProduct
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public string? Description { get; set; }

            public decimal CostPrice { get; set; }

            public decimal SellingPrice { get; set; }

            public int Quantity { get; set; }

            public int MinimumStockAlert { get; set; }
        }
    }
}
