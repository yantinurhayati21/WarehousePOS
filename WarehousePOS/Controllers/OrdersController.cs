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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<OrderResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? outletId = null,
            [FromQuery] int? customerId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = _context.Orders
                .Include(x => x.OrderType)
                .Include(x => x.Outlet)
                .Include(x => x.Customer)
                .Include(x => x.CustomerTier)
                .AsQueryable();

            if (outletId.HasValue)
                query = query.Where(x => x.OutletId == outletId.Value);

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                query = query.Where(x => x.OrderStatus == orderStatus);

            if (fromDate.HasValue)
                query = query.Where(x => x.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.OrderDate <= toDate.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderResponseDto
                {
                    OrderId = x.OrderId,
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    OrderTypeId = x.OrderTypeId,
                    OrderTypeName = x.OrderType != null ? x.OrderType.TypeName : null,
                    OutletId = x.OutletId,
                    OutletName = x.Outlet != null ? x.Outlet.OutletName : null,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer != null ? x.Customer.CustomerName : null,
                    CustomerTierId = x.CustomerTierId,
                    CustomerTierName = x.CustomerTier != null ? x.CustomerTier.TierName : null,
                    TotalItems = x.TotalItems,
                    SubtotalAmount = x.SubtotalAmount,
                    DiscountAmount = x.DiscountAmount,
                    TaxAmount = x.TaxAmount,
                    ShippingAmount = x.ShippingAmount,
                    TotalAmount = x.TotalAmount,
                    OrderStatus = x.OrderStatus.ToString(),
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<OrderResponseDto>
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(long id)
        {
            var order = await _context.Orders
                .Include(x => x.OrderType)
                .Include(x => x.Outlet)
                .Include(x => x.Customer)
                .Include(x => x.CustomerTier)
                .Include(x => x.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Include(x => x.OrderDetails)
                    .ThenInclude(d => d.Variant)
                .Include(x => x.OrderPayments)
                    .ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<OrderResponseDto>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            var response = new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                OrderTypeId = order.OrderTypeId,
                OrderTypeName = order.OrderType?.TypeName,
                OutletId = order.OutletId,
                OutletName = order.Outlet?.OutletName,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.CustomerName,
                CustomerTierId = order.CustomerTierId,
                CustomerTierName = order.CustomerTier?.TierName,
                TotalItems = order.TotalItems,
                SubtotalAmount = order.SubtotalAmount,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                ShippingAmount = order.ShippingAmount,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus.ToString(),
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.OrderDetails.Select(d => new OrderDetailResponseDto
                {
                    OrderDetailId = d.OrderDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName,
                    VariantId = d.VariantId,
                    VariantName = d.Variant?.VariantName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    DiscountPercentage = d.DiscountPercentage,
                    DiscountAmount = d.DiscountAmount,
                    Subtotal = d.Subtotal,
                    Notes = d.Notes
                }).ToList(),
                Payments = order.OrderPayments.Select(p => new OrderPaymentResponseDto
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,
                    PaymentMethodId = p.PaymentMethodId,
                    PaymentMethodName = p.PaymentMethod?.MethodName,
                    PaymentDate = p.PaymentDate,
                    PaymentAmount = p.PaymentAmount,
                    ReferenceNumber = p.ReferenceNumber,
                    PaymentStatus = p.PaymentStatus.ToString(),
                    Notes = p.Notes
                }).ToList()
            };

            return Ok(new ApiResponse<OrderResponseDto>
            {
                Success = true,
                Data = response
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Create([FromBody] OrderCreateDto dto)
        {
            // Validasi FK: OutletId
            var outlet = await _context.Outlets.FindAsync(dto.OutletId);
            if (outlet == null)
            {
                return BadRequest(new ApiResponse<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Outlet dengan ID {dto.OutletId} tidak ditemukan"
                });
            }

            // Validasi FK: OrderTypeId
            var orderType = await _context.OrderTypes.FindAsync(dto.OrderTypeId);
            if (orderType == null)
            {
                return BadRequest(new ApiResponse<OrderResponseDto>
                {
                    Success = false,
                    Message = $"OrderType dengan ID {dto.OrderTypeId} tidak ditemukan"
                });
            }

            // Validasi FK: CustomerId (jika diisi)
            Customer? customer = null;
            if (dto.CustomerId.HasValue)
            {
                customer = await _context.Customers.Include(c => c.Tier).FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId.Value && c.DeletedAt == null);
                if (customer == null)
                {
                    return BadRequest(new ApiResponse<OrderResponseDto>
                    {
                        Success = false,
                        Message = $"Customer dengan ID {dto.CustomerId.Value} tidak ditemukan"
                    });
                }
            }

            // Validasi FK: Items - ProductId dan VariantId
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.DeletedAt != null)
                {
                    return BadRequest(new ApiResponse<OrderResponseDto>
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
                        return BadRequest(new ApiResponse<OrderResponseDto>
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
                var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    OrderTypeId = dto.OrderTypeId,
                    OutletId = dto.OutletId,
                    CustomerId = dto.CustomerId,
                    CustomerTierId = customer?.TierId,
                    TotalItems = dto.Items.Count,
                    DiscountAmount = dto.DiscountAmount,
                    TaxAmount = dto.TaxAmount,
                    ShippingAmount = dto.ShippingAmount,
                    OrderStatus = OrderStatus.Draft,
                    Notes = dto.Notes,
                    CreatedBy = 1
                };

                decimal subtotal = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var item in dto.Items)
                {
                    var discountAmount = item.UnitPrice * item.Quantity * (item.DiscountPercentage / 100);
                    var itemSubtotal = (item.UnitPrice * item.Quantity) - discountAmount;
                    subtotal += itemSubtotal;

                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountPercentage = item.DiscountPercentage,
                        DiscountAmount = discountAmount,
                        Subtotal = itemSubtotal,
                        Notes = item.Notes
                    });
                }

                order.SubtotalAmount = subtotal;
                order.TotalAmount = subtotal - dto.DiscountAmount + dto.TaxAmount + dto.ShippingAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.OrderId;
                }
                _context.OrderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, new ApiResponse<OrderResponseDto>
                {
                    Success = true,
                    Message = "Order created successfully",
                    Data = new OrderResponseDto
                    {
                        OrderId = order.OrderId,
                        OrderNumber = order.OrderNumber,
                        OrderDate = order.OrderDate,
                        TotalItems = order.TotalItems,
                        SubtotalAmount = order.SubtotalAmount,
                        DiscountAmount = order.DiscountAmount,
                        TaxAmount = order.TaxAmount,
                        ShippingAmount = order.ShippingAmount,
                        TotalAmount = order.TotalAmount,
                        OrderStatus = order.OrderStatus.ToString(),
                        CreatedAt = order.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ApiResponse<OrderResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create order: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Update(long id, [FromBody] OrderUpdateDto dto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<OrderResponseDto>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            if (!string.IsNullOrEmpty(dto.OrderStatus) && Enum.TryParse<OrderStatus>(dto.OrderStatus, true, out var status))
            {
                order.OrderStatus = status;
            }
            if (dto.DiscountAmount.HasValue) order.DiscountAmount = dto.DiscountAmount.Value;
            if (dto.TaxAmount.HasValue) order.TaxAmount = dto.TaxAmount.Value;
            if (dto.ShippingAmount.HasValue) order.ShippingAmount = dto.ShippingAmount.Value;
            if (dto.Notes != null) order.Notes = dto.Notes;

            order.TotalAmount = order.SubtotalAmount - order.DiscountAmount + order.TaxAmount + order.ShippingAmount;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<OrderResponseDto>
            {
                Success = true,
                Message = "Order updated successfully"
            });
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<object>>> Cancel(long id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cannot cancel order with current status"
                });
            }

            order.OrderStatus = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = 1;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order cancelled successfully"
            });
        }
    }
}
