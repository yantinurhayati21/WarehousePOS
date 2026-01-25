using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("product_prices")]
    public class ProductPrice : BaseEntity
    {
        [Key]
        [Column("price_id")]
        public int PriceId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("price_tier_id")]
        public int PriceTierId { get; set; }

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(15,2)")]
        public decimal UnitPrice { get; set; }

        [Column("min_quantity")]
        public int MinQuantity { get; set; } = 1;

        [Required]
        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Column("end_date")]
        public DateOnly? EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }

        [ForeignKey("PriceTierId")]
        public virtual PriceTier? PriceTier { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }
    }
}
