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
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> GetById(int id)
        {
            var customer = await _context.Customers
                .Include(x => x.Tier)
                .Where(x => x.CustomerId == id && x.DeletedAt == null)
                .Select(x => new CustomerResponseDto
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    TierId = x.TierId,
                    TierName = x.Tier != null ? x.Tier.TierName : null,
                    TotalPoints = x.TotalPoints,
                    TotalPurchase = x.TotalPurchase,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();


            if (customer == null)
                throw new NotFoundException("Customer not found");

            return Ok(new ApiResponse<CustomerResponseDto>
            {
                Success = true,
                Data = customer
            });

        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Create([FromBody] CustomerCreateDto dto)
        {
            // Validasi duplicate code
            if (await _context.Customers.AnyAsync(x => x.CustomerCode == dto.CustomerCode && x.DeletedAt == null))
            {
                return BadRequest(new ApiResponse<CustomerResponseDto>
                {
                    Success = false,
                    Message = "Customer code already exists"
                });
            }

            // Validasi FK: TierId harus ada di tabel CustomerTiers jika diisi
            CustomerTier? tier = null;
            if (dto.TierId.HasValue)
            {
                tier = await _context.CustomerTiers.FindAsync(dto.TierId.Value);
                if (tier == null)
                {
                    return BadRequest(new ApiResponse<CustomerResponseDto>
                    {
                        Success = false,
                        Message = $"CustomerTier dengan ID {dto.TierId.Value} tidak ditemukan"
                    });
                }
            }

            var customer = new Customer
            {
                CustomerCode = dto.CustomerCode,
                CustomerName = dto.CustomerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                TierId = dto.TierId,
                CreatedBy = 1
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, new ApiResponse<CustomerResponseDto>
            {
                Success = true,
                Message = "Customer created successfully",
                Data = new CustomerResponseDto
                {
                    CustomerId = customer.CustomerId,
                    CustomerCode = customer.CustomerCode,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    TierId = customer.TierId,
                    TierName = tier?.TierName,
                    TotalPoints = customer.TotalPoints,
                    TotalPurchase = customer.TotalPurchase,
                    IsActive = customer.IsActive,
                    CreatedAt = customer.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            var customer = await _context.Customers
                .Include(x => x.Tier)
                .FirstOrDefaultAsync(x => x.CustomerId == id && x.DeletedAt == null);

            if (customer == null)
            {
                return NotFound(new ApiResponse<CustomerResponseDto>
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            // Validasi FK: TierId harus ada di tabel CustomerTiers jika diisi
            CustomerTier? tier = null;
            if (dto.TierId.HasValue)
            {
                tier = await _context.CustomerTiers.FindAsync(dto.TierId.Value);
                if (tier == null)
                {
                    return BadRequest(new ApiResponse<CustomerResponseDto>
                    {
                        Success = false,
                        Message = $"CustomerTier dengan ID {dto.TierId.Value} tidak ditemukan"
                    });
                }
                customer.TierId = dto.TierId.Value;
            }

            if (!string.IsNullOrEmpty(dto.CustomerName)) customer.CustomerName = dto.CustomerName;
            if (dto.Email != null) customer.Email = dto.Email;
            if (dto.Phone != null) customer.Phone = dto.Phone;
            if (dto.Address != null) customer.Address = dto.Address;
            if (dto.IsActive.HasValue) customer.IsActive = dto.IsActive.Value;
            
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            tier = customer.TierId.HasValue 
                ? await _context.CustomerTiers.FindAsync(customer.TierId.Value) 
                : null;

            return Ok(new ApiResponse<CustomerResponseDto>
            {
                Success = true,
                Message = "Customer updated successfully",
                Data = new CustomerResponseDto
                {
                    CustomerId = customer.CustomerId,
                    CustomerCode = customer.CustomerCode,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    TierId = customer.TierId,
                    TierName = tier?.TierName,
                    TotalPoints = customer.TotalPoints,
                    TotalPurchase = customer.TotalPurchase,
                    IsActive = customer.IsActive,
                    CreatedAt = customer.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id && x.DeletedAt == null);

            if (customer == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            customer.DeletedAt = DateTime.UtcNow;
            customer.DeletedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer deleted successfully"
            });
        }
    }
}
