using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("stock_logs")]
    public class StockLog
    {
        [Key]
        [Column("log_id")]
        public long LogId { get; set; }

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
        [Column("transaction_type")]
        public TransactionType TransactionType { get; set; }

        [MaxLength(100)]
        [Column("reference_id")]
        public string? ReferenceId { get; set; }

        [Column("reference_table")]
        public ReferenceTable? ReferenceTable { get; set; }

        [Required]
        [Column("quantity_before", TypeName = "decimal(15,4)")]
        public decimal QuantityBefore { get; set; }

        [Required]
        [Column("quantity_change", TypeName = "decimal(15,4)")]
        public decimal QuantityChange { get; set; }

        [Required]
        [Column("quantity_after", TypeName = "decimal(15,4)")]
        public decimal QuantityAfter { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

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
