using MiniMart.Infrastructure.Attributes;

namespace MiniMart.Application.DTO
{
    public class OrderDto
    {
        public class PurchaseProductDto
        {
            public List<ProductRequest> Products { get; set; }
        }

        public class ProductRequest
        {
            public Guid ProductId { get; set; }
            [GreaterThanZero]
            public int Quantity { get; set; }
        }

        public class OrderItemDto
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal TotalItemPrice { get; set; }
        }

        public class OrderItemResponseDto
        {
            public List<OrderItemDto> Items { get; set; }
            public decimal GrandTotal { get; set; }
        }

    }
}
