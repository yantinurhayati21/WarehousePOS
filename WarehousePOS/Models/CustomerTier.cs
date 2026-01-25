using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("customer_tiers")]
    public class CustomerTier : BaseEntity
    {
        [Key]
        [Column("tier_id")]
        public int TierId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("tier_name")]
        public string TierName { get; set; } = string.Empty;

        [Column("tier_description")]
        public string? TierDescription { get; set; }

        [Column("min_points")]
        public int MinPoints { get; set; } = 0;

        [Column("discount_percentage", TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
