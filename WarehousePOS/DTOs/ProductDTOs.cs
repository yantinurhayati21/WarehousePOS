namespace WarehousePOS.DTOs
{
    public class ProductTypeCreateDto
    {
        public string TypeName { get; set; } = string.Empty;
        public string? TypeDescription { get; set; }
    }

    public class ProductTypeResponseDto
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? TypeDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductCreateDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public int TypeId { get; set; }
        public string BaseUnit { get; set; } = string.Empty;
        public decimal ConversionFactor { get; set; } = 1;
    }

    public class ProductUpdateDto
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? TypeId { get; set; }
        public string? BaseUnit { get; set; }
        public decimal? ConversionFactor { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public int TypeId { get; set; }
        public string? TypeName { get; set; }
        public string BaseUnit { get; set; } = string.Empty;
        public decimal ConversionFactor { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductVariantCreateDto
    {
        public int ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string VariantCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public decimal? Weight { get; set; }
    }

    public class ProductVariantUpdateDto
    {
        public string? VariantName { get; set; }
        public string? Barcode { get; set; }
        public decimal? Weight { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductVariantResponseDto
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string VariantCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public decimal? Weight { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PriceTierCreateDto
    {
        public string TierName { get; set; } = string.Empty;
        public string? TierDescription { get; set; }
    }

    public class PriceTierResponseDto
    {
        public int PriceTierId { get; set; }
        public string TierName { get; set; } = string.Empty;
        public string? TierDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductPriceCreateDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int PriceTierId { get; set; }
        public int OutletId { get; set; }
        public decimal UnitPrice { get; set; }
        public int MinQuantity { get; set; } = 1;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class ProductPriceUpdateDto
    {
        public decimal? UnitPrice { get; set; }
        public int? MinQuantity { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductPriceResponseDto
    {
        public int PriceId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int PriceTierId { get; set; }
        public string? PriceTierName { get; set; }
        public int OutletId { get; set; }
        public string? OutletName { get; set; }
        public decimal UnitPrice { get; set; }
        public int MinQuantity { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
