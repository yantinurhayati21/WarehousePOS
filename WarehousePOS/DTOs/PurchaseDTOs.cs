namespace WarehousePOS.DTOs
{
    public class PurchaseCreateDto
    {
        public int SupplierId { get; set; }
        public int OutletId { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseDetailCreateDto> Items { get; set; } = new();
    }

    public class PurchaseDetailCreateDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class PurchaseUpdateDto
    {
        public string? PurchaseStatus { get; set; }
        public string? Notes { get; set; }
    }

    public class PurchaseResponseDto
    {
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PurchaseStatus { get; set; } = string.Empty;
        public DateTime? ReceivedDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseDetailResponseDto> Items { get; set; } = new();
    }

    public class PurchaseDetailResponseDto
    {
        public long PurchaseDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal { get; set; }
    }
}
