using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("so_details")]
    public class SoDetail
    {
        [Key]
        [Column("so_detail_id")]
        public long SoDetailId { get; set; }

        [Required]
        [Column("so_id")]
        public long SoId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("system_quantity", TypeName = "decimal(15,4)")]
        public decimal SystemQuantity { get; set; }

        [Required]
        [Column("physical_quantity", TypeName = "decimal(15,4)")]
        public decimal PhysicalQuantity { get; set; }

        [Required]
        [Column("variance", TypeName = "decimal(15,4)")]
        public decimal Variance { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SoId")]
        public virtual StockOpname? StockOpname { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}
