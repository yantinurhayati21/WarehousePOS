namespace WarehousePOS.DTOs
{
    public class CustomerTierCreateDto
    {
        public string TierName { get; set; } = string.Empty;
        public string? TierDescription { get; set; }
        public int MinPoints { get; set; } = 0;
        public decimal DiscountPercentage { get; set; } = 0;
    }

    public class CustomerTierUpdateDto
    {
        public string? TierName { get; set; }
        public string? TierDescription { get; set; }
        public int? MinPoints { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CustomerTierResponseDto
    {
        public int TierId { get; set; }
        public string TierName { get; set; } = string.Empty;
        public string? TierDescription { get; set; }
        public int MinPoints { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerCreateDto
    {
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? TierId { get; set; }
    }

    public class CustomerUpdateDto
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? TierId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? TierId { get; set; }
        public string? TierName { get; set; }
        public int TotalPoints { get; set; }
        public decimal TotalPurchase { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
