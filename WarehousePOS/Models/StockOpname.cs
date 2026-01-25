using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("stock_opnames")]
    public class StockOpname : BaseEntity
    {
        [Key]
        [Column("so_id")]
        public long SoId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("so_number")]
        public string SoNumber { get; set; } = string.Empty;

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Column("so_date")]
        public DateTime SoDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("total_variance", TypeName = "decimal(15,4)")]
        public decimal TotalVariance { get; set; }

        [Column("so_status")]
        public StockOpnameStatus SoStatus { get; set; } = StockOpnameStatus.Draft;

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("completed_by")]
        public int? CompletedBy { get; set; }

        // Navigation properties
        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }

        public virtual ICollection<SoDetail> SoDetails { get; set; } = new List<SoDetail>();
    }
}
