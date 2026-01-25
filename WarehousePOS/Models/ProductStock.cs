using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("product_stocks")]
    public class ProductStock
    {
        [Key]
        [Column("stock_id")]
        public int StockId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Column("location_id")]
        public int? LocationId { get; set; }

        [Required]
        [Column("quantity", TypeName = "decimal(15,4)")]
        public decimal Quantity { get; set; }

        [Column("min_stock", TypeName = "decimal(15,4)")]
        public decimal MinStock { get; set; } = 0;

        [Column("max_stock", TypeName = "decimal(15,4)")]
        public decimal MaxStock { get; set; } = 0;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }

        [ForeignKey("LocationId")]
        public virtual StockLocation? Location { get; set; }
    }
}
