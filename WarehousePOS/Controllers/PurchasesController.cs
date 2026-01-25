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
    public class PurchasesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<PurchaseResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? outletId = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = _context.Purchases
                .Include(x => x.Supplier)
                .Include(x => x.Outlet)
                .AsQueryable();

            if (outletId.HasValue)
                query = query.Where(x => x.OutletId == outletId.Value);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<PurchaseStatus>(status, true, out var purchaseStatus))
                query = query.Where(x => x.PurchaseStatus == purchaseStatus);

            if (fromDate.HasValue)
                query = query.Where(x => x.PurchaseDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.PurchaseDate <= toDate.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.PurchaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PurchaseResponseDto
                {
                    PurchaseId = x.PurchaseId,
                    PurchaseNumber = x.PurchaseNumber,
                    PurchaseDate = x.PurchaseDate,
                    SupplierId = x.SupplierId,
                    SupplierName = x.Supplier != null ? x.Supplier.SupplierName : null,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    TotalAmount = x.TotalAmount,
                    PurchaseStatus = x.PurchaseStatus.ToString(),
                    ReceivedDate = x.ReceivedDate,
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<PurchaseResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PurchaseResponseDto>>> GetById(long id)
        {
            var purchase = await _context.Purchases
                .Include(x => x.Supplier)
                .Include(x => x.Outlet)
                .Include(x => x.PurchaseDetails)
                    .ThenInclude(d => d.Product)
                .Include(x => x.PurchaseDetails)
                    .ThenInclude(d => d.Variant)
                .FirstOrDefaultAsync(x => x.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound(new ApiResponse<PurchaseResponseDto>
                {
                    Success = false,
                    Message = "Purchase not found"
                });
            }

            var response = new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                PurchaseNumber = purchase.PurchaseNumber,
                PurchaseDate = purchase.PurchaseDate,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.SupplierName,
                OutletId = purchase.OutletId,
                OutletName = purchase.Outlet?.OutletName,
                TotalAmount = purchase.TotalAmount,
                PurchaseStatus = purchase.PurchaseStatus.ToString(),
                ReceivedDate = purchase.ReceivedDate,
                Notes = purchase.Notes,
                CreatedAt = purchase.CreatedAt,
                Items = purchase.PurchaseDetails.Select(d => new PurchaseDetailResponseDto
                {
                    PurchaseDetailId = d.PurchaseDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName,
                    VariantId = d.VariantId,
                    VariantName = d.Variant?.VariantName,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost,
                    Subtotal = d.Subtotal
                }).ToList()
            };

            return Ok(new ApiResponse<PurchaseResponseDto>
            {
                Success = true,
                Data = response
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PurchaseResponseDto>>> Create([FromBody] PurchaseCreateDto dto)
        {
            // Validasi FK: SupplierId
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
            {
                return BadRequest(new ApiResponse<PurchaseResponseDto>
                {
                    Success = false,
                    Message = $"Supplier dengan ID {dto.SupplierId} tidak ditemukan"
                });
            }

            // Validasi FK: OutletId
            var outlet = await _context.Outlets.FindAsync(dto.OutletId);
            if (outlet == null)
            {
                return BadRequest(new ApiResponse<PurchaseResponseDto>
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
                    return BadRequest(new ApiResponse<PurchaseResponseDto>
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
                        return BadRequest(new ApiResponse<PurchaseResponseDto>
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
                var purchaseNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                var purchase = new Purchase
                {
                    PurchaseNumber = purchaseNumber,
                    SupplierId = dto.SupplierId,
                    OutletId = dto.OutletId,
                    PurchaseStatus = PurchaseStatus.Draft,
                    Notes = dto.Notes,
                    CreatedBy = 1
                };

                decimal totalAmount = 0;
                var purchaseDetails = new List<PurchaseDetail>();

                foreach (var item in dto.Items)
                {
                    var subtotal = item.UnitCost * item.Quantity;
                    totalAmount += subtotal;

                    purchaseDetails.Add(new PurchaseDetail
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        Subtotal = subtotal
                    });
                }

                purchase.TotalAmount = totalAmount;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                foreach (var detail in purchaseDetails)
                {
                    detail.PurchaseId = purchase.PurchaseId;
                }
                _context.PurchaseDetails.AddRange(purchaseDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = purchase.PurchaseId }, new ApiResponse<PurchaseResponseDto>
                {
                    Success = true,
                    Message = "Purchase created successfully",
                    Data = new PurchaseResponseDto
                    {
                        PurchaseId = purchase.PurchaseId,
                        PurchaseNumber = purchase.PurchaseNumber,
                        PurchaseDate = purchase.PurchaseDate,
                        TotalAmount = purchase.TotalAmount,
                        PurchaseStatus = purchase.PurchaseStatus.ToString(),
                        CreatedAt = purchase.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<PurchaseResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create purchase: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PurchaseResponseDto>>> Update(long id, [FromBody] PurchaseUpdateDto dto)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(x => x.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound(new ApiResponse<PurchaseResponseDto>
                {
                    Success = false,
                    Message = "Purchase not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.PurchaseStatus) && Enum.TryParse<PurchaseStatus>(dto.PurchaseStatus, true, out var status))
            {
                purchase.PurchaseStatus = status;
                if (status == PurchaseStatus.Received)
                {
                    purchase.ReceivedDate = DateTime.UtcNow;
                }
            }
            if (dto.Notes != null) purchase.Notes = dto.Notes;

            purchase.UpdatedAt = DateTime.UtcNow;
            purchase.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<PurchaseResponseDto>
            {
                Success = true,
                Message = "Purchase updated successfully"
            });
        }

        [HttpPost("{id}/receive")]
        public async Task<ActionResult<ApiResponse<object>>> Receive(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var purchase = await _context.Purchases
                    .Include(x => x.PurchaseDetails)
                    .FirstOrDefaultAsync(x => x.PurchaseId == id);

                if (purchase == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Purchase not found"
                    });
                }

                if (purchase.PurchaseStatus != PurchaseStatus.Ordered)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Only ordered purchases can be received"
                    });
                }

                foreach (var detail in purchase.PurchaseDetails)
                {
                    var stock = await _context.ProductStocks
                        .FirstOrDefaultAsync(s => s.ProductId == detail.ProductId 
                            && s.VariantId == detail.VariantId 
                            && s.OutletId == purchase.OutletId);

                    if (stock == null)
                    {
                        stock = new ProductStock
                        {
                            ProductId = detail.ProductId,
                            VariantId = detail.VariantId,
                            OutletId = purchase.OutletId,
                            Quantity = detail.Quantity
                        };
                        _context.ProductStocks.Add(stock);
                    }
                    else
                    {
                        var quantityBefore = stock.Quantity;
                        stock.Quantity += detail.Quantity;
                        stock.LastUpdated = DateTime.UtcNow;

                        _context.StockLogs.Add(new StockLog
                        {
                            ProductId = detail.ProductId,
                            VariantId = detail.VariantId,
                            OutletId = purchase.OutletId,
                            TransactionType = TransactionType.PO,
                            ReferenceId = purchase.PurchaseNumber,
                            ReferenceTable = ReferenceTable.Purchases,
                            QuantityBefore = quantityBefore,
                            QuantityChange = detail.Quantity,
                            QuantityAfter = stock.Quantity,
                            CreatedBy = 1
                        });
                    }
                }

                purchase.PurchaseStatus = PurchaseStatus.Received;
                purchase.ReceivedDate = DateTime.UtcNow;
                purchase.UpdatedAt = DateTime.UtcNow;
                purchase.UpdatedBy = 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Purchase received and stock updated successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to receive purchase: {ex.Message}"
                });
            }
        }
    }
}
