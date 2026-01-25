using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("order_returns")]
    public class OrderReturn : BaseEntity
    {
        [Key]
        [Column("return_id")]
        public long ReturnId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("return_number")]
        public string ReturnNumber { get; set; } = string.Empty;

        [Required]
        [Column("order_id")]
        public long OrderId { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        [Column("return_reason")]
        public string? ReturnReason { get; set; }

        [Required]
        [Column("total_refund_amount", TypeName = "decimal(15,2)")]
        public decimal TotalRefundAmount { get; set; }

        [Column("return_status")]
        public ReturnStatus ReturnStatus { get; set; } = ReturnStatus.Requested;

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        public virtual ICollection<ReturnDetail> ReturnDetails { get; set; } = new List<ReturnDetail>();
    }
}
