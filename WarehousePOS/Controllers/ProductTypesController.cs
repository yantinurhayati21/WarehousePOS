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
    public class ProductTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductTypeResponseDto>>>> GetAll([FromQuery] bool? isActive = null)
        {
            var query = _context.ProductTypes.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var items = await query
                .OrderBy(x => x.TypeName)
                .Select(x => new ProductTypeResponseDto
                {
                    TypeId = x.TypeId,
                    TypeName = x.TypeName,
                    TypeDescription = x.TypeDescription,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<ProductTypeResponseDto>>
            {
                Success = true,
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductTypeResponseDto>>> GetById(int id)
        {
            var type = await _context.ProductTypes
                .Where(x => x.TypeId == id)
                .Select(x => new ProductTypeResponseDto
                {
                    TypeId = x.TypeId,
                    TypeName = x.TypeName,
                    TypeDescription = x.TypeDescription,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (type == null)
            {
                return NotFound(new ApiResponse<ProductTypeResponseDto>
                {
                    Success = false,
                    Message = "Product type not found"
                });
            }

            return Ok(new ApiResponse<ProductTypeResponseDto>
            {
                Success = true,
                Data = type
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductTypeResponseDto>>> Create([FromBody] ProductTypeCreateDto dto)
        {
            var type = new ProductType
            {
                TypeName = dto.TypeName,
                TypeDescription = dto.TypeDescription,
                CreatedBy = 1
            };

            _context.ProductTypes.Add(type);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = type.TypeId }, new ApiResponse<ProductTypeResponseDto>
            {
                Success = true,
                Message = "Product type created successfully",
                Data = new ProductTypeResponseDto
                {
                    TypeId = type.TypeId,
                    TypeName = type.TypeName,
                    TypeDescription = type.TypeDescription,
                    IsActive = type.IsActive,
                    CreatedAt = type.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductTypeResponseDto>>> Update(int id, [FromBody] ProductTypeCreateDto dto)
        {
            var type = await _context.ProductTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new ApiResponse<ProductTypeResponseDto>
                {
                    Success = false,
                    Message = "Product type not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.TypeName)) type.TypeName = dto.TypeName;
            if (dto.TypeDescription != null) type.TypeDescription = dto.TypeDescription;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<ProductTypeResponseDto>
            {
                Success = true,
                Message = "Product type updated successfully",
                Data = new ProductTypeResponseDto
                {
                    TypeId = type.TypeId,
                    TypeName = type.TypeName,
                    TypeDescription = type.TypeDescription,
                    IsActive = type.IsActive,
                    CreatedAt = type.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var type = await _context.ProductTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product type not found"
                });
            }

            type.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product type deactivated successfully"
            });
        }
    }
}
