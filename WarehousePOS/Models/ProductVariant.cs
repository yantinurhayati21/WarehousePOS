using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("product_variants")]
    public class ProductVariant : BaseEntity
    {
        [Key]
        [Column("variant_id")]
        public int VariantId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("variant_name")]
        public string VariantName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("variant_code")]
        public string VariantCode { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("barcode")]
        public string? Barcode { get; set; }

        [Column("weight", TypeName = "decimal(10,3)")]
        public decimal? Weight { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public virtual ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public virtual ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public virtual ICollection<SoDetail> SoDetails { get; set; } = new List<SoDetail>();
        public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
    }
}
