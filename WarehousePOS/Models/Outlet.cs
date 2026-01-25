using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("outlets")]
    public class Outlet : SoftDeleteEntity
    {
        [Key]
        [Column("outlet_id")]
        public int OutletId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("outlet_code")]
        public string OutletCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("outlet_name")]
        public string OutletName { get; set; } = string.Empty;

        [Required]
        [Column("outlet_type")]
        public OutletType OutletType { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<StockLocation> StockLocations { get; set; } = new List<StockLocation>();
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public virtual ICollection<StockOpname> StockOpnames { get; set; } = new List<StockOpname>();
        public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
    }
}
