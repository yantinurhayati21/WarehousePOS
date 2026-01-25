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
    public class StockOpnamesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockOpnamesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<StockOpnameResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? outletId = null,
            [FromQuery] string? status = null)
        {
            var query = _context.StockOpnames
                .Include(x => x.Outlet)
                .AsQueryable();

            if (outletId.HasValue)
                query = query.Where(x => x.OutletId == outletId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StockOpnameStatus>(status, true, out var soStatus))
                query = query.Where(x => x.SoStatus == soStatus);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.SoDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StockOpnameResponseDto
                {
                    SoId = x.SoId,
                    SoNumber = x.SoNumber,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    SoDate = x.SoDate,
                    TotalVariance = x.TotalVariance,
                    SoStatus = x.SoStatus.ToString(),
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt,
                    CompletedAt = x.CompletedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<StockOpnameResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StockOpnameResponseDto>>> GetById(long id)
        {
            var so = await _context.StockOpnames
                .Include(x => x.Outlet)
                .Include(x => x.SoDetails)
                    .ThenInclude(d => d.Product)
                .Include(x => x.SoDetails)
                    .ThenInclude(d => d.Variant)
                .FirstOrDefaultAsync(x => x.SoId == id);

            if (so == null)
            {
                return NotFound(new ApiResponse<StockOpnameResponseDto>
                {
                    Success = false,
                    Message = "Stock opname not found"
                });
            }

            var response = new StockOpnameResponseDto
            {
                SoId = so.SoId,
                SoNumber = so.SoNumber,
                OutletId = so.OutletId,
                OutletName = so.Outlet?.OutletName,
                SoDate = so.SoDate,
                TotalVariance = so.TotalVariance,
                SoStatus = so.SoStatus.ToString(),
                Notes = so.Notes,
                CreatedAt = so.CreatedAt,
                CompletedAt = so.CompletedAt,
                Items = so.SoDetails.Select(d => new SoDetailResponseDto
                {
                    SoDetailId = d.SoDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName,
                    VariantId = d.VariantId,
                    VariantName = d.Variant?.VariantName,
                    SystemQuantity = d.SystemQuantity,
                    PhysicalQuantity = d.PhysicalQuantity,
                    Variance = d.Variance,
                    Notes = d.Notes
                }).ToList()
            };

            return Ok(new ApiResponse<StockOpnameResponseDto>
            {
                Success = true,
                Data = response
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockOpnameResponseDto>>> Create([FromBody] StockOpnameCreateDto dto)
        {
            // Validasi FK: OutletId
            var outlet = await _context.Outlets.FindAsync(dto.OutletId);
            if (outlet == null)
            {
                return BadRequest(new ApiResponse<StockOpnameResponseDto>
                {
                    Success = false,
                    Message = $"Outlet dengan ID {dto.OutletId} tidak ditemukan"
                });
            }

            // Validasi FK: Items - ProductId dan VariantId
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.DeletedAt != null)
                {
                    return BadRequest(new ApiResponse<StockOpnameResponseDto>
                    {
                        Success = false,
                        Message = $"Product dengan ID {item.ProductId} tidak ditemukan"
                    });
                }

                if (item.VariantId.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(item.VariantId.Value);
                    if (variant == null)
                    {
                        return BadRequest(new ApiResponse<StockOpnameResponseDto>
                        {
                            Success = false,
                            Message = $"ProductVariant dengan ID {item.VariantId.Value} tidak ditemukan"
                        });
                    }
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var soNumber = $"SO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                decimal totalVariance = 0;
                var soDetails = new List<SoDetail>();

                foreach (var item in dto.Items)
                {
                    var variance = item.PhysicalQuantity - item.SystemQuantity;
                    totalVariance += Math.Abs(variance);

                    soDetails.Add(new SoDetail
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        SystemQuantity = item.SystemQuantity,
                        PhysicalQuantity = item.PhysicalQuantity,
                        Variance = variance,
                        Notes = item.Notes
                    });
                }

                var so = new StockOpname
                {
                    SoNumber = soNumber,
                    OutletId = dto.OutletId,
                    TotalVariance = totalVariance,
                    SoStatus = StockOpnameStatus.Draft,
                    Notes = dto.Notes,
                    CreatedBy = 1
                };

                _context.StockOpnames.Add(so);
                await _context.SaveChangesAsync();

                foreach (var detail in soDetails)
                {
                    detail.SoId = so.SoId;
                }
                _context.SoDetails.AddRange(soDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = so.SoId }, new ApiResponse<StockOpnameResponseDto>
                {
                    Success = true,
                    Message = "Stock opname created successfully",
                    Data = new StockOpnameResponseDto
                    {
                        SoId = so.SoId,
                        SoNumber = so.SoNumber,
                        OutletId = so.OutletId,
                        SoDate = so.SoDate,
                        TotalVariance = so.TotalVariance,
                        SoStatus = so.SoStatus.ToString(),
                        CreatedAt = so.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<StockOpnameResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create stock opname: {ex.Message}"
                });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<ApiResponse<object>>> Complete(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var so = await _context.StockOpnames
                    .Include(x => x.SoDetails)
                    .FirstOrDefaultAsync(x => x.SoId == id);

                if (so == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Stock opname not found"
                    });
                }

                if (so.SoStatus == StockOpnameStatus.Completed)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Stock opname already completed"
                    });
                }

                foreach (var detail in so.SoDetails)
                {
                    var stock = await _context.ProductStocks
                        .FirstOrDefaultAsync(s => s.ProductId == detail.ProductId 
                            && s.VariantId == detail.VariantId 
                            && s.OutletId == so.OutletId);

                    if (stock != null && detail.Variance != 0)
                    {
                        var quantityBefore = stock.Quantity;
                        stock.Quantity = detail.PhysicalQuantity;
                        stock.LastUpdated = DateTime.UtcNow;

                        _context.StockLogs.Add(new StockLog
                        {
                            ProductId = detail.ProductId,
                            VariantId = detail.VariantId,
                            OutletId = so.OutletId,
                            TransactionType = TransactionType.SO,
                            ReferenceId = so.SoNumber,
                            ReferenceTable = ReferenceTable.StockOpnames,
                            QuantityBefore = quantityBefore,
                            QuantityChange = detail.Variance,
                            QuantityAfter = detail.PhysicalQuantity,
                            Notes = $"Stock opname adjustment: {detail.Notes}",
                            CreatedBy = 1
                        });
                    }
                }

                so.SoStatus = StockOpnameStatus.Completed;
                so.CompletedAt = DateTime.UtcNow;
                so.CompletedBy = 1;
                so.UpdatedAt = DateTime.UtcNow;
                so.UpdatedBy = 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Stock opname completed and stock updated successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to complete stock opname: {ex.Message}"
                });
            }
        }
    }
}
