using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("price_tiers")]
    public class PriceTier
    {
        [Key]
        [Column("price_tier_id")]
        public int PriceTierId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("tier_name")]
        public string TierName { get; set; } = string.Empty;

        [Column("tier_description")]
        public string? TierDescription { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
    }
}
