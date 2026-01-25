using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("products")]
    public class Product : SoftDeleteEntity
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("product_code")]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("product_description")]
        public string? ProductDescription { get; set; }

        [Required]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [Column("base_unit")]
        public BaseUnit BaseUnit { get; set; }

        [Column("conversion_factor", TypeName = "decimal(10,4)")]
        public decimal ConversionFactor { get; set; } = 1;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("TypeId")]
        public virtual ProductType? Type { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public virtual ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public virtual ICollection<SoDetail> SoDetails { get; set; } = new List<SoDetail>();
        public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
    }
}
