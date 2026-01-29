using System;
using System.ComponentModel.DataAnnotations;

namespace WarehousePOS.DTOs
{
    // =========================
    // CREATE DTO
    // =========================
    public class OutletCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string OutletCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string OutletName { get; set; } = string.Empty;

        [Required]
        public string OutletType { get; set; } = string.Empty;
        // Warehouse | Store | Branch

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }

    // =========================
    // UPDATE DTO
    // =========================
    public class OutletUpdateDto
    {
        [MaxLength(100)]
        public string? OutletName { get; set; }

        public string? OutletType { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public bool? IsActive { get; set; }
    }

    // =========================
    // RESPONSE DTO
    // =========================
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

    // =========================
    // QUERY / FILTER DTO
    // =========================
    public class OutletQueryDto
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filter
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public string? OutletType { get; set; }

        // Sorting
        public string SortBy { get; set; } = "OutletName";
        public string SortDir { get; set; } = "asc"; // asc | desc

        // Date range
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
