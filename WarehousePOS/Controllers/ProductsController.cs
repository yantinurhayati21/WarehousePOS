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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ProductResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? typeId = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.Products
                .Include(x => x.Type)
                .Where(x => x.DeletedAt == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.ProductName.Contains(search) || x.ProductCode.Contains(search));
            }

            if (typeId.HasValue)
            {
                query = query.Where(x => x.TypeId == typeId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductResponseDto
                {
                    ProductId = x.ProductId,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ProductDescription = x.ProductDescription,
                    TypeId = x.TypeId,
                    TypeName = x.Type != null ? x.Type.TypeName : null,
                    BaseUnit = x.BaseUnit.ToString(),
                    ConversionFactor = x.ConversionFactor,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<ProductResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
        {
            var product = await _context.Products
                .Include(x => x.Type)
                .Where(x => x.ProductId == id && x.DeletedAt == null)
                .Select(x => new ProductResponseDto
                {
                    ProductId = x.ProductId,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ProductDescription = x.ProductDescription,
                    TypeId = x.TypeId,
                    TypeName = x.Type != null ? x.Type.TypeName : null,
                    BaseUnit = x.BaseUnit.ToString(),
                    ConversionFactor = x.ConversionFactor,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<ProductResponseDto>
            {
                Success = true,
                Data = product
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] ProductCreateDto dto)
        {
            // Validasi duplicate code
            if (await _context.Products.AnyAsync(x => x.ProductCode == dto.ProductCode && x.DeletedAt == null))
            {
                return BadRequest(new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    Message = "Product code already exists"
                });
            }

            // Validasi BaseUnit enum
            if (!Enum.TryParse<BaseUnit>(dto.BaseUnit, true, out var baseUnit))
            {
                return BadRequest(new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    Message = "Invalid base unit. Valid values: Kg, Gram, Pcs, Pack"
                });
            }

            // Validasi FK: TypeId harus ada di tabel ProductTypes
            var productType = await _context.ProductTypes.FindAsync(dto.TypeId);
            if (productType == null)
            {
                return BadRequest(new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    Message = $"ProductType dengan ID {dto.TypeId} tidak ditemukan"
                });
            }

            var product = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                TypeId = dto.TypeId,
                BaseUnit = baseUnit,
                ConversionFactor = dto.ConversionFactor,
                CreatedBy = 1
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new ApiResponse<ProductResponseDto>
            {
                Success = true,
                Message = "Product created successfully",
                Data = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    TypeId = product.TypeId,
                    TypeName = productType?.TypeName,
                    BaseUnit = product.BaseUnit.ToString(),
                    ConversionFactor = product.ConversionFactor,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _context.Products
                .Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

            if (product == null)
            {
                return NotFound(new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            // Validasi FK: TypeId harus ada di tabel ProductTypes jika diisi
            if (dto.TypeId.HasValue)
            {
                var type = await _context.ProductTypes.FindAsync(dto.TypeId.Value);
                if (type == null)
                {
                    return BadRequest(new ApiResponse<ProductResponseDto>
                    {
                        Success = false,
                        Message = $"ProductType dengan ID {dto.TypeId.Value} tidak ditemukan"
                    });
                }
                product.TypeId = dto.TypeId.Value;
            }

            if (!string.IsNullOrEmpty(dto.ProductName)) product.ProductName = dto.ProductName;
            if (dto.ProductDescription != null) product.ProductDescription = dto.ProductDescription;
            if (!string.IsNullOrEmpty(dto.BaseUnit))
            {
                if (Enum.TryParse<BaseUnit>(dto.BaseUnit, true, out var baseUnit))
                {
                    product.BaseUnit = baseUnit;
                }
            }
            if (dto.ConversionFactor.HasValue) product.ConversionFactor = dto.ConversionFactor.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;
            
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            var productType = await _context.ProductTypes.FindAsync(product.TypeId);

            return Ok(new ApiResponse<ProductResponseDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    TypeId = product.TypeId,
                    TypeName = productType?.TypeName,
                    BaseUnit = product.BaseUnit.ToString(),
                    ConversionFactor = product.ConversionFactor,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            // Cek apakah ada ProductVariant yang menggunakan product ini
            var hasVariants = await _context.ProductVariants.AnyAsync(v => v.ProductId == id);
            if (hasVariants)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tidak dapat menghapus product karena masih memiliki variant"
                });
            }

            product.DeletedAt = DateTime.UtcNow;
            product.DeletedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }
    }
}
