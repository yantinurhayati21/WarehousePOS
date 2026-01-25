using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("customers")]
    public class Customer : SoftDeleteEntity
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("customer_code")]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("tier_id")]
        public int? TierId { get; set; }

        [Column("total_points")]
        public int TotalPoints { get; set; } = 0;

        [Column("total_purchase", TypeName = "decimal(15,2)")]
        public decimal TotalPurchase { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("TierId")]
        public virtual CustomerTier? Tier { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
