namespace WarehousePOS.DTOs
{
    public class OrderReturnCreateDto
    {
        public long OrderId { get; set; }
        public string? ReturnReason { get; set; }
        public List<ReturnDetailCreateDto> Items { get; set; } = new();
    }

    public class ReturnDetailCreateDto
    {
        public long OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Reason { get; set; }
    }

    public class OrderReturnUpdateDto
    {
        public string? ReturnStatus { get; set; }
    }

    public class OrderReturnResponseDto
    {
        public long ReturnId { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public long OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? ReturnReason { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string ReturnStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ReturnDetailResponseDto> Items { get; set; } = new();
    }

    public class ReturnDetailResponseDto
    {
        public long ReturnDetailId { get; set; }
        public long OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
    }
}
