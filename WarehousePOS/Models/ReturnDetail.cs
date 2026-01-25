using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("return_details")]
    public class ReturnDetail
    {
        [Key]
        [Column("return_detail_id")]
        public long ReturnDetailId { get; set; }

        [Required]
        [Column("return_id")]
        public long ReturnId { get; set; }

        [Required]
        [Column("order_detail_id")]
        public long OrderDetailId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("return_quantity", TypeName = "decimal(10,4)")]
        public decimal ReturnQuantity { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(15,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("refund_amount", TypeName = "decimal(15,2)")]
        public decimal RefundAmount { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        // Navigation properties
        [ForeignKey("ReturnId")]
        public virtual OrderReturn? OrderReturn { get; set; }

        [ForeignKey("OrderDetailId")]
        public virtual OrderDetail? OrderDetail { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}
