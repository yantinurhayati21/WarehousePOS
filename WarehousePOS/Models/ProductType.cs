using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehousePOS.Models
{
    [Table("product_types")]
    public class ProductType
    {
        [Key]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("type_name")]
        public string TypeName { get; set; } = string.Empty;

        [Column("type_description")]
        public string? TypeDescription { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
