using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMart.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }

        public Order Order { get; set; }


        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }

}