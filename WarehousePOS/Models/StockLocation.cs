using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("stock_locations")]
    public class StockLocation
    {
        [Key]
        [Column("location_id")]
        public int LocationId { get; set; }

        [Required]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("location_name")]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("location_code")]
        public string LocationCode { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("OutletId")]
        public virtual Outlet? Outlet { get; set; }

        public virtual ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
    }
}
