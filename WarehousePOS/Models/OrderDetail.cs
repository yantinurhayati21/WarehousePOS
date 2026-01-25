using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("order_details")]
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        public long OrderDetailId { get; set; }

        [Required]
        [Column("order_id")]
        public long OrderId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Required]
        [Column("quantity", TypeName = "decimal(10,4)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(15,2)")]
        public decimal UnitPrice { get; set; }

        [Column("discount_percentage", TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column("discount_amount", TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column("subtotal", TypeName = "decimal(15,2)")]
        public decimal Subtotal { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }

        public virtual ICollection<ReturnDetail> ReturnDetails { get; set; } = new List<ReturnDetail>();
    }
}
