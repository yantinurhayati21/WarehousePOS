namespace WarehousePOS.DTOs
{
    public class StockOpnameCreateDto
    {
        public int OutletId { get; set; }
        public string? Notes { get; set; }
        public List<SoDetailCreateDto> Items { get; set; } = new();
    }

    public class SoDetailCreateDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public string? Notes { get; set; }
    }

    public class StockOpnameUpdateDto
    {
        public string? SoStatus { get; set; }
        public string? Notes { get; set; }
    }

    public class StockOpnameResponseDto
    {
        public long SoId { get; set; }
        public string SoNumber { get; set; } = string.Empty;
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public DateTime SoDate { get; set; }
        public decimal TotalVariance { get; set; }
        public string SoStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<SoDetailResponseDto> Items { get; set; } = new();
    }

    public class SoDetailResponseDto
    {
        public long SoDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public string? Notes { get; set; }
    }
}
