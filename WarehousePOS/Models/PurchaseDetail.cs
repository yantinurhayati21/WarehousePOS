using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("purchase_details")]
    public class PurchaseDetail
    {
        [Key]
        [Column("purchase_detail_id")]
        public long PurchaseDetailId { get; set; }

        [Required]
        [Column("purchase_id")]
        public long PurchaseId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("quantity", TypeName = "decimal(10,4)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column("unit_cost", TypeName = "decimal(15,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column("subtotal", TypeName = "decimal(15,2)")]
        public decimal Subtotal { get; set; }

        // Navigation properties
        [ForeignKey("PurchaseId")]
        public virtual Purchase? Purchase { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}
