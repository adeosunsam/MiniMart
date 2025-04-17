using System.ComponentModel.DataAnnotations;

namespace MiniMart.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } 
        public string? ModifiedBy { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
