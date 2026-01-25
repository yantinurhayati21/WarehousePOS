using WarehousePOS.Models;

namespace WarehousePOS.DTOs
{
    public class OutletCreateDto
    {
        public string OutletCode { get; set; } = string.Empty;
        public string OutletName { get; set; } = string.Empty;
        public string OutletType { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class OutletUpdateDto
    {
        public string? OutletName { get; set; }
        public string? OutletType { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OutletResponseDto
    {
        public int OutletId { get; set; }
        public string OutletCode { get; set; } = string.Empty;
        public string OutletName { get; set; } = string.Empty;
        public string OutletType { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
