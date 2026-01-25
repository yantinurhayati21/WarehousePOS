namespace WarehousePOS.DTOs
{
    public class OrderTypeCreateDto
    {
        public string TypeCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class OrderTypeResponseDto
    {
        public int OrderTypeId { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentMethodCreateDto
    {
        public string MethodCode { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class PaymentMethodResponseDto
    {
        public int MethodId { get; set; }
        public string MethodCode { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OrderCreateDto
    {
        public int OrderTypeId { get; set; }
        public int OutletId { get; set; }
        public int? CustomerId { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal ShippingAmount { get; set; } = 0;
        public string? Notes { get; set; }
        public List<OrderDetailCreateDto> Items { get; set; } = new();
    }

    public class OrderDetailCreateDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public string? Notes { get; set; }
    }

    public class OrderUpdateDto
    {
        public string? OrderStatus { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ShippingAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderResponseDto
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int OrderTypeId { get; set; }
        public string? OrderTypeName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? CustomerTierId { get; set; }
        public string? CustomerTierName { get; set; }
        public int TotalItems { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailResponseDto> Items { get; set; } = new();
        public List<OrderPaymentResponseDto> Payments { get; set; } = new();
    }

    public class OrderDetailResponseDto
    {
        public long OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Subtotal { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderPaymentCreateDto
    {
        public long OrderId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderPaymentResponseDto
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
