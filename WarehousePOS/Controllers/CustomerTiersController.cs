using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehousePOS.Data;
using WarehousePOS.DTOs;
using WarehousePOS.Models;

namespace WarehousePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "ExternalToken")]
    public class CustomerTiersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerTiersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CustomerTierResponseDto>>>> GetAll([FromQuery] bool? isActive = null)
        {
            var query = _context.CustomerTiers.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var items = await query
                .OrderBy(x => x.MinPoints)
                .Select(x => new CustomerTierResponseDto
                {
                    TierId = x.TierId,
                    TierName = x.TierName,
                    TierDescription = x.TierDescription,
                    MinPoints = x.MinPoints,
                    DiscountPercentage = x.DiscountPercentage,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<CustomerTierResponseDto>>
            {
                Success = true,
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerTierResponseDto>>> GetById(int id)
        {
            var tier = await _context.CustomerTiers
                .Where(x => x.TierId == id)
                .Select(x => new CustomerTierResponseDto
                {
                    TierId = x.TierId,
                    TierName = x.TierName,
                    TierDescription = x.TierDescription,
                    MinPoints = x.MinPoints,
                    DiscountPercentage = x.DiscountPercentage,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (tier == null)
            {
                return NotFound(new ApiResponse<CustomerTierResponseDto>
                {
                    Success = false,
                    Message = "Customer tier not found"
                });
            }

            return Ok(new ApiResponse<CustomerTierResponseDto>
            {
                Success = true,
                Data = tier
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerTierResponseDto>>> Create([FromBody] CustomerTierCreateDto dto)
        {
            var tier = new CustomerTier
            {
                TierName = dto.TierName,
                TierDescription = dto.TierDescription,
                MinPoints = dto.MinPoints,
                DiscountPercentage = dto.DiscountPercentage,
                CreatedBy = 1
            };

            _context.CustomerTiers.Add(tier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = tier.TierId }, new ApiResponse<CustomerTierResponseDto>
            {
                Success = true,
                Message = "Customer tier created successfully",
                Data = new CustomerTierResponseDto
                {
                    TierId = tier.TierId,
                    TierName = tier.TierName,
                    TierDescription = tier.TierDescription,
                    MinPoints = tier.MinPoints,
                    DiscountPercentage = tier.DiscountPercentage,
                    IsActive = tier.IsActive,
                    CreatedAt = tier.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerTierResponseDto>>> Update(int id, [FromBody] CustomerTierUpdateDto dto)
        {
            var tier = await _context.CustomerTiers.FindAsync(id);

            if (tier == null)
            {
                return NotFound(new ApiResponse<CustomerTierResponseDto>
                {
                    Success = false,
                    Message = "Customer tier not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.TierName)) tier.TierName = dto.TierName;
            if (dto.TierDescription != null) tier.TierDescription = dto.TierDescription;
            if (dto.MinPoints.HasValue) tier.MinPoints = dto.MinPoints.Value;
            if (dto.DiscountPercentage.HasValue) tier.DiscountPercentage = dto.DiscountPercentage.Value;
            if (dto.IsActive.HasValue) tier.IsActive = dto.IsActive.Value;
            
            tier.UpdatedAt = DateTime.UtcNow;
            tier.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<CustomerTierResponseDto>
            {
                Success = true,
                Message = "Customer tier updated successfully",
                Data = new CustomerTierResponseDto
                {
                    TierId = tier.TierId,
                    TierName = tier.TierName,
                    TierDescription = tier.TierDescription,
                    MinPoints = tier.MinPoints,
                    DiscountPercentage = tier.DiscountPercentage,
                    IsActive = tier.IsActive,
                    CreatedAt = tier.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var tier = await _context.CustomerTiers.FindAsync(id);

            if (tier == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer tier not found"
                });
            }

            // Cek apakah ada Customer yang menggunakan tier ini
            var hasCustomers = await _context.Customers.AnyAsync(c => c.TierId == id && c.DeletedAt == null);
            if (hasCustomers)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tidak dapat menghapus tier karena masih digunakan oleh customer"
                });
            }

            tier.IsActive = false;
            tier.UpdatedAt = DateTime.UtcNow;
            tier.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer tier deactivated successfully"
            });
        }
    }
}
