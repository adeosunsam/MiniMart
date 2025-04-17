namespace MiniMart.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }

        public int MinimumStockAlert { get; set; }
    }
}
