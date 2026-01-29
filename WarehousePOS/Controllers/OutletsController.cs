using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehousePOS.Data;
using WarehousePOS.DTOs;
using WarehousePOS.Exceptions;
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
    [FromQuery] OutletQueryDto dto)
        {
            var query = _context.Outlets
                .Where(x => x.DeletedAt == null)
                .AsQueryable();

            // =========================
            // SEARCH
            // =========================
            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(x =>
                    x.OutletName.Contains(dto.Search) ||
                    x.OutletCode.Contains(dto.Search));
            }

            // =========================
            // IS ACTIVE
            // =========================
            if (dto.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == dto.IsActive.Value);
            }

            // =========================
            // OUTLET TYPE
            // =========================
            if (!string.IsNullOrEmpty(dto.OutletType) &&
                Enum.TryParse<OutletType>(dto.OutletType, true, out var outletType))
            {
                query = query.Where(x => x.OutletType == outletType);
            }

            // =========================
            // DATE RANGE
            // =========================
            if (dto.StartDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= dto.StartDate.Value);
            }

            if (dto.EndDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= dto.EndDate.Value);
            }

            // =========================
            // SORTING
            // =========================
            query = dto.SortBy.ToLower() switch
            {
                "outletcode" => dto.SortDir == "desc"
                    ? query.OrderByDescending(x => x.OutletCode)
                    : query.OrderBy(x => x.OutletCode),

                "createdat" => dto.SortDir == "desc"
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),

                _ => dto.SortDir == "desc"
                    ? query.OrderByDescending(x => x.OutletName)
                    : query.OrderBy(x => x.OutletName)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
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
                Page = dto.Page,
                PageSize = dto.PageSize
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
        public async Task<ActionResult<ApiResponse<OutletResponseDto>>> Create(
    [FromBody] OutletCreateDto dto)
        {
            // 1?? Body null
            if (dto == null)
                throw new BadRequestException("Request body cannot be null");

            // 2?? DataAnnotation validation
            if (!ModelState.IsValid)
                throw new BadRequestException(
                    string.Join("; ",
                        ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));

            // 3?? Trim input
            dto.OutletCode = dto.OutletCode.Trim();
            dto.OutletName = dto.OutletName.Trim();
            dto.OutletType = dto.OutletType.Trim();

            // 4?? Empty string manual check
            if (string.IsNullOrWhiteSpace(dto.OutletCode))
                throw new BadRequestException("Outlet code is required");

            if (string.IsNullOrWhiteSpace(dto.OutletName))
                throw new BadRequestException("Outlet name is required");

            // 5?? Validate OutletType enum
            if (!Enum.TryParse<OutletType>(dto.OutletType, true, out var outletType))
                throw new BadRequestException(
                    "Invalid outlet type. Valid values: Warehouse, Store, Branch");

            // 6?? Duplicate check (case-insensitive)
            var isExist = await _context.Outlets.AnyAsync(x =>
                x.DeletedAt == null &&
                x.OutletCode.ToLower() == dto.OutletCode.ToLower());

            if (isExist)
                throw new BadRequestException("Outlet code already exists");

            // 7?? Create entity
            var outlet = new Outlet
            {
                OutletCode = dto.OutletCode,
                OutletName = dto.OutletName,
                OutletType = outletType,
                Address = dto.Address?.Trim(),
                Phone = dto.Phone?.Trim(),
                Email = dto.Email?.Trim(),
                CreatedBy = 1
            };

            _context.Outlets.Add(outlet);
            await _context.SaveChangesAsync();

            // 8?? Response
            return CreatedAtAction(nameof(GetById),
                new { id = outlet.OutletId },
                new ApiResponse<OutletResponseDto>
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
