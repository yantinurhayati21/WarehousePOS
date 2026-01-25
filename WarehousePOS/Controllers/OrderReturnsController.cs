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
    public class OrderReturnsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderReturnsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OrderReturnResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? orderId = null,
            [FromQuery] string? status = null)
        {
            var query = _context.OrderReturns
                .Include(x => x.Order)
                .AsQueryable();

            if (orderId.HasValue)
                query = query.Where(x => x.OrderId == orderId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReturnStatus>(status, true, out var returnStatus))
                query = query.Where(x => x.ReturnStatus == returnStatus);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.ReturnDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderReturnResponseDto
                {
                    ReturnId = x.ReturnId,
                    ReturnNumber = x.ReturnNumber,
                    OrderId = x.OrderId,
                    OrderNumber = x.Order != null ? x.Order.OrderNumber : null,
                    ReturnDate = x.ReturnDate,
                    ReturnReason = x.ReturnReason,
                    TotalRefundAmount = x.TotalRefundAmount,
                    ReturnStatus = x.ReturnStatus.ToString(),
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<OrderReturnResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderReturnResponseDto>>> GetById(long id)
        {
            var orderReturn = await _context.OrderReturns
                .Include(x => x.Order)
                .Include(x => x.ReturnDetails)
                    .ThenInclude(d => d.Product)
                .Include(x => x.ReturnDetails)
                    .ThenInclude(d => d.Variant)
                .FirstOrDefaultAsync(x => x.ReturnId == id);

            if (orderReturn == null)
            {
                return NotFound(new ApiResponse<OrderReturnResponseDto>
                {
                    Success = false,
                    Message = "Order return not found"
                });
            }

            var response = new OrderReturnResponseDto
            {
                ReturnId = orderReturn.ReturnId,
                ReturnNumber = orderReturn.ReturnNumber,
                OrderId = orderReturn.OrderId,
                OrderNumber = orderReturn.Order?.OrderNumber,
                ReturnDate = orderReturn.ReturnDate,
                ReturnReason = orderReturn.ReturnReason,
                TotalRefundAmount = orderReturn.TotalRefundAmount,
                ReturnStatus = orderReturn.ReturnStatus.ToString(),
                CreatedAt = orderReturn.CreatedAt,
                Items = orderReturn.ReturnDetails.Select(d => new ReturnDetailResponseDto
                {
                    ReturnDetailId = d.ReturnDetailId,
                    OrderDetailId = d.OrderDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName,
                    VariantId = d.VariantId,
                    VariantName = d.Variant?.VariantName,
                    ReturnQuantity = d.ReturnQuantity,
                    UnitPrice = d.UnitPrice,
                    RefundAmount = d.RefundAmount,
                    Reason = d.Reason
                }).ToList()
            };

            return Ok(new ApiResponse<OrderReturnResponseDto>
            {
                Success = true,
                Data = response
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderReturnResponseDto>>> Create([FromBody] OrderReturnCreateDto dto)
        {
            // Validasi FK: OrderId
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null)
            {
                return BadRequest(new ApiResponse<OrderReturnResponseDto>
                {
                    Success = false,
                    Message = $"Order dengan ID {dto.OrderId} tidak ditemukan"
                });
            }

            // Validasi: Order harus sudah Delivered/Completed untuk bisa di-return
            if (order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Draft)
            {
                return BadRequest(new ApiResponse<OrderReturnResponseDto>
                {
                    Success = false,
                    Message = "Tidak dapat membuat return untuk order dengan status Draft atau Cancelled"
                });
            }

            // Validasi FK: Items - ProductId dan VariantId
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.DeletedAt != null)
                {
                    return BadRequest(new ApiResponse<OrderReturnResponseDto>
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
                        return BadRequest(new ApiResponse<OrderReturnResponseDto>
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
                var returnNumber = $"RET-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                decimal totalRefund = 0;
                var returnDetails = new List<ReturnDetail>();

                foreach (var item in dto.Items)
                {
                    var refundAmount = item.UnitPrice * item.ReturnQuantity;
                    totalRefund += refundAmount;

                    returnDetails.Add(new ReturnDetail
                    {
                        OrderDetailId = item.OrderDetailId,
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        ReturnQuantity = item.ReturnQuantity,
                        UnitPrice = item.UnitPrice,
                        RefundAmount = refundAmount,
                        Reason = item.Reason
                    });
                }

                var orderReturn = new OrderReturn
                {
                    ReturnNumber = returnNumber,
                    OrderId = dto.OrderId,
                    ReturnReason = dto.ReturnReason,
                    TotalRefundAmount = totalRefund,
                    ReturnStatus = ReturnStatus.Requested,
                    CreatedBy = 1
                };

                _context.OrderReturns.Add(orderReturn);
                await _context.SaveChangesAsync();

                foreach (var detail in returnDetails)
                {
                    detail.ReturnId = orderReturn.ReturnId;
                }
                _context.ReturnDetails.AddRange(returnDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = orderReturn.ReturnId }, new ApiResponse<OrderReturnResponseDto>
                {
                    Success = true,
                    Message = "Order return created successfully",
                    Data = new OrderReturnResponseDto
                    {
                        ReturnId = orderReturn.ReturnId,
                        ReturnNumber = orderReturn.ReturnNumber,
                        OrderId = orderReturn.OrderId,
                        ReturnDate = orderReturn.ReturnDate,
                        TotalRefundAmount = orderReturn.TotalRefundAmount,
                        ReturnStatus = orderReturn.ReturnStatus.ToString(),
                        CreatedAt = orderReturn.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<OrderReturnResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create order return: {ex.Message}"
                });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApiResponse<object>>> Approve(long id)
        {
            var orderReturn = await _context.OrderReturns.FindAsync(id);

            if (orderReturn == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order return not found"
                });
            }

            if (orderReturn.ReturnStatus != ReturnStatus.Requested)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Only requested returns can be approved"
                });
            }

            orderReturn.ReturnStatus = ReturnStatus.Approved;
            orderReturn.UpdatedAt = DateTime.UtcNow;
            orderReturn.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order return approved successfully"
            });
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<ApiResponse<object>>> Complete(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var orderReturn = await _context.OrderReturns
                    .Include(x => x.Order)
                    .Include(x => x.ReturnDetails)
                    .FirstOrDefaultAsync(x => x.ReturnId == id);

                if (orderReturn == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Order return not found"
                    });
                }

                if (orderReturn.ReturnStatus != ReturnStatus.Approved)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Only approved returns can be completed"
                    });
                }

                foreach (var detail in orderReturn.ReturnDetails)
                {
                    var stock = await _context.ProductStocks
                        .FirstOrDefaultAsync(s => s.ProductId == detail.ProductId 
                            && s.VariantId == detail.VariantId 
                            && s.OutletId == orderReturn.Order!.OutletId);

                    if (stock != null)
                    {
                        var quantityBefore = stock.Quantity;
                        stock.Quantity += detail.ReturnQuantity;
                        stock.LastUpdated = DateTime.UtcNow;

                        _context.StockLogs.Add(new StockLog
                        {
                            ProductId = detail.ProductId,
                            VariantId = detail.VariantId,
                            OutletId = orderReturn.Order!.OutletId,
                            TransactionType = TransactionType.RETURN,
                            ReferenceId = orderReturn.ReturnNumber,
                            ReferenceTable = ReferenceTable.Returns,
                            QuantityBefore = quantityBefore,
                            QuantityChange = detail.ReturnQuantity,
                            QuantityAfter = stock.Quantity,
                            Notes = $"Return: {detail.Reason}",
                            CreatedBy = 1
                        });
                    }
                }

                orderReturn.ReturnStatus = ReturnStatus.Completed;
                orderReturn.UpdatedAt = DateTime.UtcNow;
                orderReturn.UpdatedBy = 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Order return completed and stock updated successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to complete order return: {ex.Message}"
                });
            }
        }
    }
}
