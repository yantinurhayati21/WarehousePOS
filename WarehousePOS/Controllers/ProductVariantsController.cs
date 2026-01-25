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
    public class ProductVariantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductVariantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ProductVariantResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? productId = null,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.ProductVariants
                .Include(x => x.Product)
                .AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.VariantName.Contains(search) || x.VariantCode.Contains(search) || (x.Barcode != null && x.Barcode.Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.VariantName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductVariantResponseDto
                {
                    VariantId = x.VariantId,
                    ProductId = x.ProductId,
                    ProductName = x.Product != null ? x.Product.ProductName : null,
                    VariantName = x.VariantName,
                    VariantCode = x.VariantCode,
                    Barcode = x.Barcode,
                    Weight = x.Weight,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<ProductVariantResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductVariantResponseDto>>> GetById(int id)
        {
            var variant = await _context.ProductVariants
                .Include(x => x.Product)
                .Where(x => x.VariantId == id)
                .Select(x => new ProductVariantResponseDto
                {
                    VariantId = x.VariantId,
                    ProductId = x.ProductId,
                    ProductName = x.Product != null ? x.Product.ProductName : null,
                    VariantName = x.VariantName,
                    VariantCode = x.VariantCode,
                    Barcode = x.Barcode,
                    Weight = x.Weight,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (variant == null)
            {
                return NotFound(new ApiResponse<ProductVariantResponseDto>
                {
                    Success = false,
                    Message = "Product variant not found"
                });
            }

            return Ok(new ApiResponse<ProductVariantResponseDto>
            {
                Success = true,
                Data = variant
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductVariantResponseDto>>> Create([FromBody] ProductVariantCreateDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || product.DeletedAt != null)
            {
                return BadRequest(new ApiResponse<ProductVariantResponseDto>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            var variant = new ProductVariant
            {
                ProductId = dto.ProductId,
                VariantName = dto.VariantName,
                VariantCode = dto.VariantCode,
                Barcode = dto.Barcode,
                Weight = dto.Weight,
                CreatedBy = 1
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = variant.VariantId }, new ApiResponse<ProductVariantResponseDto>
            {
                Success = true,
                Message = "Product variant created successfully",
                Data = new ProductVariantResponseDto
                {
                    VariantId = variant.VariantId,
                    ProductId = variant.ProductId,
                    ProductName = product.ProductName,
                    VariantName = variant.VariantName,
                    VariantCode = variant.VariantCode,
                    Barcode = variant.Barcode,
                    Weight = variant.Weight,
                    IsActive = variant.IsActive,
                    CreatedAt = variant.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductVariantResponseDto>>> Update(int id, [FromBody] ProductVariantUpdateDto dto)
        {
            var variant = await _context.ProductVariants
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.VariantId == id);

            if (variant == null)
            {
                return NotFound(new ApiResponse<ProductVariantResponseDto>
                {
                    Success = false,
                    Message = "Product variant not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.VariantName)) variant.VariantName = dto.VariantName;
            if (dto.Barcode != null) variant.Barcode = dto.Barcode;
            if (dto.Weight.HasValue) variant.Weight = dto.Weight.Value;
            if (dto.IsActive.HasValue) variant.IsActive = dto.IsActive.Value;
            
            variant.UpdatedAt = DateTime.UtcNow;
            variant.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<ProductVariantResponseDto>
            {
                Success = true,
                Message = "Product variant updated successfully",
                Data = new ProductVariantResponseDto
                {
                    VariantId = variant.VariantId,
                    ProductId = variant.ProductId,
                    ProductName = variant.Product?.ProductName,
                    VariantName = variant.VariantName,
                    VariantCode = variant.VariantCode,
                    Barcode = variant.Barcode,
                    Weight = variant.Weight,
                    IsActive = variant.IsActive,
                    CreatedAt = variant.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);

            if (variant == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product variant not found"
                });
            }

            // Cek apakah variant ini digunakan di ProductStock
            var hasStock = await _context.ProductStocks.AnyAsync(s => s.VariantId == id);
            if (hasStock)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tidak dapat menghapus variant karena masih memiliki data stock"
                });
            }

            // Cek apakah variant ini digunakan di OrderDetail
            var hasOrders = await _context.OrderDetails.AnyAsync(od => od.VariantId == id);
            if (hasOrders)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tidak dapat menghapus variant karena sudah digunakan dalam transaksi order"
                });
            }

            variant.IsActive = false;
            variant.UpdatedAt = DateTime.UtcNow;
            variant.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product variant deactivated successfully"
            });
        }
    }
}
