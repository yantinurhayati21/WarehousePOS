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
    public class OrderTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderTypeResponseDto>>>> GetAll([FromQuery] bool? isActive = null)
        {
            var query = _context.OrderTypes.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var items = await query
                .OrderBy(x => x.TypeName)
                .Select(x => new OrderTypeResponseDto
                {
                    OrderTypeId = x.OrderTypeId,
                    TypeCode = x.TypeCode,
                    TypeName = x.TypeName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<OrderTypeResponseDto>>
            {
                Success = true,
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderTypeResponseDto>>> GetById(int id)
        {
            var type = await _context.OrderTypes
                .Where(x => x.OrderTypeId == id)
                .Select(x => new OrderTypeResponseDto
                {
                    OrderTypeId = x.OrderTypeId,
                    TypeCode = x.TypeCode,
                    TypeName = x.TypeName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (type == null)
            {
                return NotFound(new ApiResponse<OrderTypeResponseDto>
                {
                    Success = false,
                    Message = "Order type not found"
                });
            }

            return Ok(new ApiResponse<OrderTypeResponseDto>
            {
                Success = true,
                Data = type
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderTypeResponseDto>>> Create([FromBody] OrderTypeCreateDto dto)
        {
            var type = new OrderType
            {
                TypeCode = dto.TypeCode,
                TypeName = dto.TypeName,
                Description = dto.Description,
                CreatedBy = 1
            };

            _context.OrderTypes.Add(type);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = type.OrderTypeId }, new ApiResponse<OrderTypeResponseDto>
            {
                Success = true,
                Message = "Order type created successfully",
                Data = new OrderTypeResponseDto
                {
                    OrderTypeId = type.OrderTypeId,
                    TypeCode = type.TypeCode,
                    TypeName = type.TypeName,
                    Description = type.Description,
                    IsActive = type.IsActive,
                    CreatedAt = type.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var type = await _context.OrderTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order type not found"
                });
            }

            type.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order type deactivated successfully"
            });
        }
    }
}
