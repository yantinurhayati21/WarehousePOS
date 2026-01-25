using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("payment_methods")]
    public class PaymentMethod
    {
        [Key]
        [Column("method_id")]
        public int MethodId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("method_code")]
        public string MethodCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("method_name")]
        public string MethodName { get; set; } = string.Empty;

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
        public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
    }
}
