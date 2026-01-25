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
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<SupplierResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.Suppliers.Where(x => x.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.SupplierName.Contains(search) || x.SupplierCode.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.SupplierName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SupplierResponseDto
                {
                    SupplierId = x.SupplierId,
                    SupplierCode = x.SupplierCode,
                    SupplierName = x.SupplierName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<SupplierResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> GetById(int id)
        {
            var supplier = await _context.Suppliers
                .Where(x => x.SupplierId == id && x.DeletedAt == null)
                .Select(x => new SupplierResponseDto
                {
                    SupplierId = x.SupplierId,
                    SupplierCode = x.SupplierCode,
                    SupplierName = x.SupplierName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound(new ApiResponse<SupplierResponseDto>
                {
                    Success = false,
                    Message = "Supplier not found"
                });
            }

            return Ok(new ApiResponse<SupplierResponseDto>
            {
                Success = true,
                Data = supplier
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Create([FromBody] SupplierCreateDto dto)
        {
            if (await _context.Suppliers.AnyAsync(x => x.SupplierCode == dto.SupplierCode && x.DeletedAt == null))
            {
                return BadRequest(new ApiResponse<SupplierResponseDto>
                {
                    Success = false,
                    Message = "Supplier code already exists"
                });
            }

            var supplier = new Supplier
            {
                SupplierCode = dto.SupplierCode,
                SupplierName = dto.SupplierName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedBy = 1
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, new ApiResponse<SupplierResponseDto>
            {
                Success = true,
                Message = "Supplier created successfully",
                Data = new SupplierResponseDto
                {
                    SupplierId = supplier.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier.SupplierName,
                    Email = supplier.Email,
                    Phone = supplier.Phone,
                    Address = supplier.Address,
                    IsActive = supplier.IsActive,
                    CreatedAt = supplier.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Update(int id, [FromBody] SupplierUpdateDto dto)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id && x.DeletedAt == null);

            if (supplier == null)
            {
                return NotFound(new ApiResponse<SupplierResponseDto>
                {
                    Success = false,
                    Message = "Supplier not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.SupplierName)) supplier.SupplierName = dto.SupplierName;
            if (dto.Email != null) supplier.Email = dto.Email;
            if (dto.Phone != null) supplier.Phone = dto.Phone;
            if (dto.Address != null) supplier.Address = dto.Address;
            if (dto.IsActive.HasValue) supplier.IsActive = dto.IsActive.Value;
            
            supplier.UpdatedAt = DateTime.UtcNow;
            supplier.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<SupplierResponseDto>
            {
                Success = true,
                Message = "Supplier updated successfully",
                Data = new SupplierResponseDto
                {
                    SupplierId = supplier.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier.SupplierName,
                    Email = supplier.Email,
                    Phone = supplier.Phone,
                    Address = supplier.Address,
                    IsActive = supplier.IsActive,
                    CreatedAt = supplier.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id && x.DeletedAt == null);

            if (supplier == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Supplier not found"
                });
            }

            supplier.DeletedAt = DateTime.UtcNow;
            supplier.DeletedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Supplier deleted successfully"
            });
        }
    }
}
