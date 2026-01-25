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
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StocksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ProductStockResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? outletId = null,
            [FromQuery] int? productId = null,
            [FromQuery] bool? lowStock = null)
        {
            var query = _context.ProductStocks
                .Include(x => x.Product)
                .Include(x => x.Variant)
                .Include(x => x.Outlet)
                .Include(x => x.Location)
                .AsQueryable();

            if (outletId.HasValue)
                query = query.Where(x => x.OutletId == outletId.Value);

            if (productId.HasValue)
                query = query.Where(x => x.ProductId == productId.Value);

            if (lowStock == true)
                query = query.Where(x => x.Quantity <= x.MinStock);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Product!.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductStockResponseDto
                {
                    StockId = x.StockId,
                    ProductId = x.ProductId,
                    ProductName = x.Product != null ? x.Product.ProductName : null,
                    VariantId = x.VariantId,
                    VariantName = x.Variant != null ? x.Variant.VariantName : null,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    LocationId = x.LocationId,
                    LocationName = x.Location != null ? x.Location.LocationName : null,
                    Quantity = x.Quantity,
                    MinStock = x.MinStock,
                    MaxStock = x.MaxStock,
                    LastUpdated = x.LastUpdated
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<ProductStockResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductStockResponseDto>>> GetById(int id)
        {
            var stock = await _context.ProductStocks
                .Include(x => x.Product)
                .Include(x => x.Variant)
                .Include(x => x.Outlet)
                .Include(x => x.Location)
                .Where(x => x.StockId == id)
                .Select(x => new ProductStockResponseDto
                {
                    StockId = x.StockId,
                    ProductId = x.ProductId,
                    ProductName = x.Product != null ? x.Product.ProductName : null,
                    VariantId = x.VariantId,
                    VariantName = x.Variant != null ? x.Variant.VariantName : null,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    LocationId = x.LocationId,
                    LocationName = x.Location != null ? x.Location.LocationName : null,
                    Quantity = x.Quantity,
                    MinStock = x.MinStock,
                    MaxStock = x.MaxStock,
                    LastUpdated = x.LastUpdated
                })
                .FirstOrDefaultAsync();

            if (stock == null)
            {
                return NotFound(new ApiResponse<ProductStockResponseDto>
                {
                    Success = false,
                    Message = "Stock not found"
                });
            }

            return Ok(new ApiResponse<ProductStockResponseDto>
            {
                Success = true,
                Data = stock
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductStockResponseDto>>> Create([FromBody] ProductStockCreateDto dto)
        {
            // Validasi FK: ProductId
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || product.DeletedAt != null)
            {
                return BadRequest(new ApiResponse<ProductStockResponseDto>
                {
                    Success = false,
                    Message = $"Product dengan ID {dto.ProductId} tidak ditemukan"
                });
            }

            // Validasi FK: VariantId (jika diisi)
            if (dto.VariantId.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(dto.VariantId.Value);
                if (variant == null)
                {
                    return BadRequest(new ApiResponse<ProductStockResponseDto>
                    {
                        Success = false,
                        Message = $"ProductVariant dengan ID {dto.VariantId.Value} tidak ditemukan"
                    });
                }
            }

            // Validasi FK: OutletId
            var outlet = await _context.Outlets.FindAsync(dto.OutletId);
            if (outlet == null)
            {
                return BadRequest(new ApiResponse<ProductStockResponseDto>
                {
                    Success = false,
                    Message = $"Outlet dengan ID {dto.OutletId} tidak ditemukan"
                });
            }

            // Validasi FK: LocationId (jika diisi)
            if (dto.LocationId.HasValue)
            {
                var location = await _context.StockLocations.FindAsync(dto.LocationId.Value);
                if (location == null)
                {
                    return BadRequest(new ApiResponse<ProductStockResponseDto>
                    {
                        Success = false,
                        Message = $"StockLocation dengan ID {dto.LocationId.Value} tidak ditemukan"
                    });
                }
            }

            // Cek duplicate
            var existingStock = await _context.ProductStocks
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId 
                    && x.VariantId == dto.VariantId 
                    && x.OutletId == dto.OutletId
                    && x.LocationId == dto.LocationId);

            if (existingStock != null)
            {
                return BadRequest(new ApiResponse<ProductStockResponseDto>
                {
                    Success = false,
                    Message = "Stock record already exists for this product/variant/outlet/location combination"
                });
            }

            var stock = new ProductStock
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                OutletId = dto.OutletId,
                LocationId = dto.LocationId,
                Quantity = dto.Quantity,
                MinStock = dto.MinStock,
                MaxStock = dto.MaxStock
            };

            _context.ProductStocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stock.StockId }, new ApiResponse<ProductStockResponseDto>
            {
                Success = true,
                Message = "Stock created successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductStockResponseDto>>> Update(int id, [FromBody] ProductStockUpdateDto dto)
        {
            var stock = await _context.ProductStocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound(new ApiResponse<ProductStockResponseDto>
                {
                    Success = false,
                    Message = "Stock not found"
                });
            }

            if (dto.MinStock.HasValue) stock.MinStock = dto.MinStock.Value;
            if (dto.MaxStock.HasValue) stock.MaxStock = dto.MaxStock.Value;
            
            stock.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<ProductStockResponseDto>
            {
                Success = true,
                Message = "Stock updated successfully"
            });
        }

        [HttpPost("adjust")]
        public async Task<ActionResult<ApiResponse<object>>> Adjust([FromBody] StockAdjustmentDto dto)
        {
            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId 
                    && x.VariantId == dto.VariantId 
                    && x.OutletId == dto.OutletId
                    && x.LocationId == dto.LocationId);

            if (stock == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Stock not found"
                });
            }

            var quantityBefore = stock.Quantity;
            stock.Quantity += dto.QuantityChange;
            stock.LastUpdated = DateTime.UtcNow;

            _context.StockLogs.Add(new StockLog
            {
                ProductId = dto.ProductId,
                VariantId = dto.VariantId,
                OutletId = dto.OutletId,
                LocationId = dto.LocationId,
                TransactionType = TransactionType.ADJUSTMENT,
                QuantityBefore = quantityBefore,
                QuantityChange = dto.QuantityChange,
                QuantityAfter = stock.Quantity,
                Notes = dto.Notes,
                CreatedBy = 1
            });

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock adjusted successfully"
            });
        }

        [HttpGet("logs")]
        public async Task<ActionResult<PaginatedResponse<StockLogResponseDto>>> GetLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? outletId = null,
            [FromQuery] int? productId = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = _context.StockLogs
                .Include(x => x.Product)
                .Include(x => x.Variant)
                .Include(x => x.Outlet)
                .Include(x => x.Location)
                .AsQueryable();

            if (outletId.HasValue)
                query = query.Where(x => x.OutletId == outletId.Value);

            if (productId.HasValue)
                query = query.Where(x => x.ProductId == productId.Value);

            if (!string.IsNullOrEmpty(transactionType) && Enum.TryParse<TransactionType>(transactionType, true, out var type))
                query = query.Where(x => x.TransactionType == type);

            if (fromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.CreatedAt <= toDate.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StockLogResponseDto
                {
                    LogId = x.LogId,
                    ProductId = x.ProductId,
                    ProductName = x.Product != null ? x.Product.ProductName : null,
                    VariantId = x.VariantId,
                    VariantName = x.Variant != null ? x.Variant.VariantName : null,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    LocationId = x.LocationId,
                    LocationName = x.Location != null ? x.Location.LocationName : null,
                    TransactionType = x.TransactionType.ToString(),
                    ReferenceId = x.ReferenceId,
                    ReferenceTable = x.ReferenceTable.HasValue ? x.ReferenceTable.Value.ToString() : null,
                    QuantityBefore = x.QuantityBefore,
                    QuantityChange = x.QuantityChange,
                    QuantityAfter = x.QuantityAfter,
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<StockLogResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}
