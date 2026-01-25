using System.ComponentModel.DataAnnotations;

namespace WarehousePOS.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public abstract class SoftDeleteEntity : BaseEntity
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}
