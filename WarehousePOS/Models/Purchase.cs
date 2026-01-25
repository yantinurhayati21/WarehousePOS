using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("purchases")]
    public class Purchase : BaseEntity
    {
        [Key]
        [Column("purchase_id")]
        public long PurchaseId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("purchase_number")]
        public string PurchaseNumber { get; set; } = string.Empty;

        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Required]
        [Column("total_amount", TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        [Column("purchase_status")]
        public PurchaseStatus PurchaseStatus { get; set; } = PurchaseStatus.Draft;

        [Column("received_date")]
        public DateTime? ReceivedDate { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }

        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
