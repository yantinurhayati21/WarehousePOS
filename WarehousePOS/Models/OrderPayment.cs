using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("order_payments")]
    public class OrderPayment : BaseEntity
    {
        [Key]
        [Column("payment_id")]
        public long PaymentId { get; set; }

        [Required]
        [Column("order_id")]
        public long OrderId { get; set; }

        [Required]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("payment_amount", TypeName = "decimal(15,2)")]
        public decimal PaymentAmount { get; set; }

        [MaxLength(100)]
        [Column("reference_number")]
        public string? ReferenceNumber { get; set; }

        [Column("payment_status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }
    }
}
