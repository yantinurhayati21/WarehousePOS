namespace WarehousePOS.DTOs
{
    public class StockLocationCreateDto
    {
        public int OutletId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
    }

    public class StockLocationResponseDto
    {
        public int LocationId { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductStockCreateDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int OutletId { get; set; }
        public int? LocationId { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinStock { get; set; } = 0;
        public decimal MaxStock { get; set; } = 0;
    }

    public class ProductStockUpdateDto
    {
        public decimal? Quantity { get; set; }
        public decimal? MinStock { get; set; }
        public decimal? MaxStock { get; set; }
    }

    public class ProductStockResponseDto
    {
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public int? LocationId { get; set; }
        public string? LocationName { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class StockAdjustmentDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int OutletId { get; set; }
        public int? LocationId { get; set; }
        public decimal QuantityChange { get; set; }
        public string? Notes { get; set; }
    }

    public class StockLogResponseDto
    {
        public long LogId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public int? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? ReferenceId { get; set; }
        public string? ReferenceTable { get; set; }
        public decimal QuantityBefore { get; set; }
        public decimal QuantityChange { get; set; }
        public decimal QuantityAfter { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
