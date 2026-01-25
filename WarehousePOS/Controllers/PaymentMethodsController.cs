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
    public class PaymentMethodsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentMethodsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PaymentMethodResponseDto>>>> GetAll([FromQuery] bool? isActive = null)
        {
            var query = _context.PaymentMethods.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var items = await query
                .OrderBy(x => x.MethodName)
                .Select(x => new PaymentMethodResponseDto
                {
                    MethodId = x.MethodId,
                    MethodCode = x.MethodCode,
                    MethodName = x.MethodName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<PaymentMethodResponseDto>>
            {
                Success = true,
                Data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PaymentMethodResponseDto>>> GetById(int id)
        {
            var method = await _context.PaymentMethods
                .Where(x => x.MethodId == id)
                .Select(x => new PaymentMethodResponseDto
                {
                    MethodId = x.MethodId,
                    MethodCode = x.MethodCode,
                    MethodName = x.MethodName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (method == null)
            {
                return NotFound(new ApiResponse<PaymentMethodResponseDto>
                {
                    Success = false,
                    Message = "Payment method not found"
                });
            }

            return Ok(new ApiResponse<PaymentMethodResponseDto>
            {
                Success = true,
                Data = method
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PaymentMethodResponseDto>>> Create([FromBody] PaymentMethodCreateDto dto)
        {
            var method = new PaymentMethod
            {
                MethodCode = dto.MethodCode,
                MethodName = dto.MethodName,
                Description = dto.Description,
                CreatedBy = 1
            };

            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = method.MethodId }, new ApiResponse<PaymentMethodResponseDto>
            {
                Success = true,
                Message = "Payment method created successfully",
                Data = new PaymentMethodResponseDto
                {
                    MethodId = method.MethodId,
                    MethodCode = method.MethodCode,
                    MethodName = method.MethodName,
                    Description = method.Description,
                    IsActive = method.IsActive,
                    CreatedAt = method.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var method = await _context.PaymentMethods.FindAsync(id);

            if (method == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Payment method not found"
                });
            }

            method.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Payment method deactivated successfully"
            });
        }
    }
}
