using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("order_types")]
    public class OrderType
    {
        [Key]
        [Column("order_type_id")]
        public int OrderTypeId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("type_code")]
        public string TypeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("type_name")]
        public string TypeName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
