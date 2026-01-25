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
    public class OutletsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OutletsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OutletResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.Outlets.Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.OutletName.Contains(search) || x.OutletCode.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OutletName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OutletResponseDto
                {
                    OutletId = x.OutletId,
                    OutletCode = x.OutletCode,
                    OutletName = x.OutletName,
                    OutletType = x.OutletType.ToString(),
                    Address = x.Address,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<OutletResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OutletResponseDto>>> GetById(int id)
        {
            var outlet = await _context.Outlets
                .Where(x => x.OutletId == id && x.DeletedAt == null)
                .Select(x => new OutletResponseDto
                {
                    OutletId = x.OutletId,
                    OutletCode = x.OutletCode,
                    OutletName = x.OutletName,
                    OutletType = x.OutletType.ToString(),
                    Address = x.Address,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (outlet == null)
            {
                return NotFound(new ApiResponse<OutletResponseDto>
                {
                    Success = false,
                    Message = "Outlet not found"
                });
            }

            return Ok(new ApiResponse<OutletResponseDto>
            {
                Success = true,
                Data = outlet
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OutletResponseDto>>> Create([FromBody] OutletCreateDto dto)
        {
            if (await _context.Outlets.AnyAsync(x => x.OutletCode == dto.OutletCode && x.DeletedAt == null))
            {
                return BadRequest(new ApiResponse<OutletResponseDto>
                {
                    Success = false,
                    Message = "Outlet code already exists"
                });
            }

            if (!Enum.TryParse<OutletType>(dto.OutletType, true, out var outletType))
            {
                return BadRequest(new ApiResponse<OutletResponseDto>
                {
                    Success = false,
                    Message = "Invalid outlet type. Valid values: Warehouse, Store, Branch"
                });
            }

            var outlet = new Outlet
            {
                OutletCode = dto.OutletCode,
                OutletName = dto.OutletName,
                OutletType = outletType,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                CreatedBy = 1
            };

            _context.Outlets.Add(outlet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = outlet.OutletId }, new ApiResponse<OutletResponseDto>
            {
                Success = true,
                Message = "Outlet created successfully",
                Data = new OutletResponseDto
                {
                    OutletId = outlet.OutletId,
                    OutletCode = outlet.OutletCode,
                    OutletName = outlet.OutletName,
                    OutletType = outlet.OutletType.ToString(),
                    Address = outlet.Address,
                    Phone = outlet.Phone,
                    Email = outlet.Email,
                    IsActive = outlet.IsActive,
                    CreatedAt = outlet.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<OutletResponseDto>>> Update(int id, [FromBody] OutletUpdateDto dto)
        {
            var outlet = await _context.Outlets.FirstOrDefaultAsync(x => x.OutletId == id && x.DeletedAt == null);

            if (outlet == null)
            {
                return NotFound(new ApiResponse<OutletResponseDto>
                {
                    Success = false,
                    Message = "Outlet not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.OutletName)) outlet.OutletName = dto.OutletName;
            if (!string.IsNullOrEmpty(dto.OutletType))
            {
                if (Enum.TryParse<OutletType>(dto.OutletType, true, out var outletType))
                {
                    outlet.OutletType = outletType;
                }
            }
            if (dto.Address != null) outlet.Address = dto.Address;
            if (dto.Phone != null) outlet.Phone = dto.Phone;
            if (dto.Email != null) outlet.Email = dto.Email;
            if (dto.IsActive.HasValue) outlet.IsActive = dto.IsActive.Value;
            
            outlet.UpdatedAt = DateTime.UtcNow;
            outlet.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<OutletResponseDto>
            {
                Success = true,
                Message = "Outlet updated successfully",
                Data = new OutletResponseDto
                {
                    OutletId = outlet.OutletId,
                    OutletCode = outlet.OutletCode,
                    OutletName = outlet.OutletName,
                    OutletType = outlet.OutletType.ToString(),
                    Address = outlet.Address,
                    Phone = outlet.Phone,
                    Email = outlet.Email,
                    IsActive = outlet.IsActive,
                    CreatedAt = outlet.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var outlet = await _context.Outlets.FirstOrDefaultAsync(x => x.OutletId == id && x.DeletedAt == null);

            if (outlet == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Outlet not found"
                });
            }

            outlet.DeletedAt = DateTime.UtcNow;
            outlet.DeletedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Outlet deleted successfully"
            });
        }
    }
}
