using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("orders")]
    public class Order : BaseEntity
    {
        [Key]
        [Column("order_id")]
        public long OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("order_type_id")]
        public int OrderTypeId { get; set; }

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("customer_tier_id")]
        public int? CustomerTierId { get; set; }

        [Required]
        [Column("total_items")]
        public int TotalItems { get; set; }

        [Required]
        [Column("subtotal_amount", TypeName = "decimal(15,2)")]
        public decimal SubtotalAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("tax_amount", TypeName = "decimal(15,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("shipping_amount", TypeName = "decimal(15,2)")]
        public decimal ShippingAmount { get; set; } = 0;

        [Required]
        [Column("total_amount", TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        [Column("order_status")]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Draft;

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("OrderTypeId")]
        public virtual OrderType? OrderType { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("CustomerTierId")]
        public virtual CustomerTier? CustomerTier { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
        public virtual ICollection<OrderReturn> OrderReturns { get; set; } = new List<OrderReturn>();
    }
}
