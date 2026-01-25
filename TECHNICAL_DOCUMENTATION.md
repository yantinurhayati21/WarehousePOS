# Dokumentasi Teknis - Warehouse & POS System

## 1. Arsitektur Sistem

### 1.1 Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                             │
│  (Postman / Frontend App / Mobile App)                          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP/HTTPS
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      AUTHENTICATION                              │
│  ┌─────────────────┐              ┌─────────────────┐           │
│  │  WIT External   │◄────────────►│ Local API       │           │
│  │  API            │   Token      │ (Auth Handler)  │           │
│  └─────────────────┘              └─────────────────┘           │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ Validated Request
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      APPLICATION LAYER                           │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │ Controllers │  │  Services   │  │    DTOs     │              │
│  └─────────────┘  └─────────────┘  └─────────────┘              │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ Entity Framework Core
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        DATA LAYER                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │ AppDbContext│  │   Models    │  │ SQL Server  │              │
│  └─────────────┘  └─────────────┘  └─────────────┘              │
└─────────────────────────────────────────────────────────────────┘
```

### 1.2 Technology Stack

| Layer | Technology | Version |
|-------|------------|---------|
| Runtime | .NET | 8.0 |
| Web Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0.8 |
| Database | SQL Server | 2019+ |
| API Documentation | Swashbuckle (Swagger) | 6.4.0 |
| Authentication | Custom Handler + External API | - |

## 2. Authentication System

### 2.1 External Token Authentication

Sistem menggunakan custom `AuthenticationHandler` untuk memvalidasi token dari external WIT API.

#### Flow Diagram

```
┌──────────┐                    ┌──────────┐                    ┌──────────┐
│  Client  │                    │ WIT API  │                    │Local API │
└────┬─────┘                    └────┬─────┘                    └────┬─────┘
     │                               │                               │
     │ 1. POST /GenerateToken        │                               │
     │──────────────────────────────►│                               │
     │                               │                               │
     │ 2. Return: { token: "xxx" }   │                               │
     │◄──────────────────────────────│                               │
     │                               │                               │
     │ 3. GET /api/Outlets           │                               │
     │   Header: token: xxx          │                               │
     │──────────────────────────────────────────────────────────────►│
     │                               │                               │
     │                               │    4. Validate token exists   │
     │                               │    (not empty)                │
     │                               │                               │
     │ 5. Return: [outlets data]     │                               │
     │◄──────────────────────────────────────────────────────────────│
     │                               │                               │
```

#### ExternalTokenAuthHandler

```csharp
public class ExternalTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1. Check header "token" atau "Authorization"
        string? token = null;
        
        if (Request.Headers.TryGetValue("token", out var tokenHeader))
        {
            token = tokenHeader.ToString();
        }
        else if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var authValue = authHeader.ToString();
            token = authValue.StartsWith("Bearer ") 
                ? authValue.Substring(7).Trim() 
                : authValue.Trim();
        }

        // 2. Validasi token tidak kosong
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Token required"));
        }

        // 3. Create authenticated principal
        var claims = new[] { new Claim(ClaimTypes.Name, "ExternalUser") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

### 2.2 Request Headers

| Header | Format | Contoh |
|--------|--------|--------|
| `token` | Plain token | `token: abc123xyz` |
| `Authorization` | Bearer token | `Authorization: Bearer abc123xyz` |

## 3. Database Design

### 3.1 Entity Relationship Diagram

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│     Outlet      │       │    Customer     │       │  CustomerTier   │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │───┐   │ Id (PK)         │       │ Id (PK)         │
│ Code            │   │   │ Code            │   ┌───│ Name            │
│ Name            │   │   │ Name            │   │   │ DiscountPercent │
│ Type            │   └──►│ OutletId (FK)   │   │   └─────────────────┘
│ Address         │       │ TierId (FK)     │◄──┘
└─────────────────┘       └─────────────────┘
        │
        │
        ▼
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│  ProductStock   │       │ ProductVariant  │       │    Product      │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │       │ Id (PK)         │       │ Id (PK)         │
│ OutletId (FK)   │◄──┐   │ SKU             │   ┌───│ Code            │
│ VariantId (FK)  │───┼──►│ Name            │   │   │ Name            │
│ LocationId (FK) │   │   │ ProductId (FK)  │◄──┘   │ TypeId (FK)     │
│ Quantity        │   │   │ BasePrice       │       └────────┬────────┘
└─────────────────┘   │   └─────────────────┘                │
                      │                                       ▼
                      │                              ┌─────────────────┐
                      │                              │   ProductType   │
                      │                              ├─────────────────┤
                      │                              │ Id (PK)         │
                      │                              │ Code            │
                      │                              │ Name            │
                      │                              └─────────────────┘
                      │
┌─────────────────┐   │   ┌─────────────────┐       ┌─────────────────┐
│     Order       │   │   │   OrderDetail   │       │  OrderPayment   │
├─────────────────┤   │   ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │───┼──►│ Id (PK)         │       │ Id (PK)         │
│ OrderNumber     │   │   │ OrderId (FK)    │◄──┐   │ OrderId (FK)    │◄─┐
│ OutletId (FK)   │◄──┘   │ VariantId (FK)  │   │   │ MethodId (FK)   │  │
│ CustomerId (FK) │       │ Quantity        │   │   │ Amount          │  │
│ TotalAmount     │       │ UnitPrice       │   └───┼──────────────────  │
└────────┬────────┘       └─────────────────┘       └─────────────────┘  │
         │                                                               │
         └───────────────────────────────────────────────────────────────┘
```

### 3.2 Table Definitions

#### Core Tables

**outlets**
| Column | Type | Constraint | Description |
|--------|------|------------|-------------|
| id | int | PK, Identity | Primary key |
| code | varchar(20) | Unique, Not Null | Kode outlet |
| name | varchar(100) | Not Null | Nama outlet |
| type | varchar(20) | Not Null | Enum: Warehouse, Store |
| address | varchar(500) | | Alamat |
| city | varchar(100) | | Kota |
| phone | varchar(20) | | Telepon |
| created_at | datetime2 | Not Null | Timestamp create |
| updated_at | datetime2 | | Timestamp update |

**customers**
| Column | Type | Constraint | Description |
|--------|------|------------|-------------|
| id | int | PK, Identity | Primary key |
| code | varchar(20) | Unique, Not Null | Kode customer |
| name | varchar(100) | Not Null | Nama customer |
| outlet_id | int | FK → outlets | Outlet terdaftar |
| tier_id | int | FK → customer_tiers | Tier pricing |
| email | varchar(100) | | Email |
| phone | varchar(20) | | Telepon |
| address | varchar(500) | | Alamat |

**products**
| Column | Type | Constraint | Description |
|--------|------|------------|-------------|
| id | int | PK, Identity | Primary key |
| code | varchar(20) | Unique, Not Null | Kode produk |
| name | varchar(100) | Not Null | Nama produk |
| type_id | int | FK → product_types | Kategori |
| base_unit | varchar(20) | Not Null | Enum: Pcs, Box, Kg, etc |
| description | varchar(500) | | Deskripsi |

**product_variants**
| Column | Type | Constraint | Description |
|--------|------|------------|-------------|
| id | int | PK, Identity | Primary key |
| sku | varchar(50) | Unique, Not Null | Stock Keeping Unit |
| name | varchar(100) | Not Null | Nama varian |
| product_id | int | FK → products | Parent product |
| base_price | decimal(18,2) | Not Null | Harga dasar |
| cost_price | decimal(18,2) | | Harga beli |

**orders**
| Column | Type | Constraint | Description |
|--------|------|------------|-------------|
| id | int | PK, Identity | Primary key |
| order_number | varchar(50) | Unique, Not Null | Nomor order |
| outlet_id | int | FK → outlets | Outlet transaksi |
| customer_id | int | FK → customers | Customer (nullable) |
| order_type_id | int | FK → order_types | Tipe order |
| order_date | datetime2 | Not Null | Tanggal order |
| subtotal | decimal(18,2) | | Subtotal |
| discount_amount | decimal(18,2) | | Diskon |
| tax_amount | decimal(18,2) | | Pajak |
| total_amount | decimal(18,2) | | Total |
| status | varchar(20) | | Enum: Pending, Completed, Cancelled |
| payment_status | varchar(20) | | Enum: Unpaid, Partial, Paid |

### 3.3 Indexes

```sql
-- Unique indexes
CREATE UNIQUE INDEX IX_outlets_code ON outlets(code);
CREATE UNIQUE INDEX IX_customers_code ON customers(code);
CREATE UNIQUE INDEX IX_products_code ON products(code);
CREATE UNIQUE INDEX IX_product_variants_sku ON product_variants(sku);
CREATE UNIQUE INDEX IX_orders_order_number ON orders(order_number);

-- Foreign key indexes
CREATE INDEX IX_customers_outlet_id ON customers(outlet_id);
CREATE INDEX IX_customers_tier_id ON customers(tier_id);
CREATE INDEX IX_products_type_id ON products(type_id);
CREATE INDEX IX_product_variants_product_id ON product_variants(product_id);
CREATE INDEX IX_orders_outlet_id ON orders(outlet_id);
CREATE INDEX IX_orders_customer_id ON orders(customer_id);
CREATE INDEX IX_order_details_order_id ON order_details(order_id);
```

## 4. API Design

### 4.1 Response Format

#### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

#### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detail error 1", "Detail error 2"]
}
```

#### Paginated Response
```json
{
  "success": true,
  "data": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

### 4.2 HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation error |
| 401 | Unauthorized | Missing/invalid token |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

### 4.3 Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "ExternalToken")]
public class OutletsController : ControllerBase
{
    private readonly AppDbContext _context;

    // GET: api/Outlets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutletResponse>>> GetAll()
    {
        var outlets = await _context.Outlets
            .Select(o => new OutletResponse { ... })
            .ToListAsync();
        return Ok(outlets);
    }

    // GET: api/Outlets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OutletResponse>> GetById(int id)
    {
        var outlet = await _context.Outlets.FindAsync(id);
        if (outlet == null) return NotFound();
        return Ok(new OutletResponse { ... });
    }

    // POST: api/Outlets
    [HttpPost]
    public async Task<ActionResult<OutletResponse>> Create(CreateOutletRequest request)
    {
        var outlet = new Outlet { ... };
        _context.Outlets.Add(outlet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = outlet.Id }, new OutletResponse { ... });
    }

    // PUT: api/Outlets/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateOutletRequest request)
    {
        var outlet = await _context.Outlets.FindAsync(id);
        if (outlet == null) return NotFound();
        // Update properties
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Outlets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var outlet = await _context.Outlets.FindAsync(id);
        if (outlet == null) return NotFound();
        _context.Outlets.Remove(outlet);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

## 5. Entity Models

### 5.1 Base Entity

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public abstract class SoftDeleteEntity : BaseEntity
{
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted => DeletedAt.HasValue;
}
```

### 5.2 Enums

```csharp
public enum OutletType { Warehouse, Store }
public enum BaseUnit { Pcs, Box, Pack, Kg, Gram, Liter, Ml }
public enum TransactionType { In, Out, Adjustment, Transfer }
public enum OrderStatus { Pending, Processing, Completed, Cancelled }
public enum PaymentStatus { Unpaid, Partial, Paid, Refunded }
public enum ReturnStatus { Pending, Approved, Rejected, Completed }
public enum PurchaseStatus { Draft, Ordered, Received, Cancelled }
public enum StockOpnameStatus { Draft, InProgress, Completed, Cancelled }
```

## 6. Configuration

### 6.1 Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient for external API
builder.Services.AddHttpClient<IAuthService, AuthService>();

// Custom Authentication
builder.Services.AddAuthentication("ExternalToken")
    .AddScheme<AuthenticationSchemeOptions, ExternalTokenAuthHandler>("ExternalToken", null);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { ... });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 6.2 DbContext Configuration

```csharp
public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraints
        modelBuilder.Entity<Outlet>()
            .HasIndex(o => o.Code)
            .IsUnique();

        // Enum to string conversion
        modelBuilder.Entity<Outlet>()
            .Property(o => o.Type)
            .HasConversion<string>();

        // Relationships
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Outlet)
            .WithMany(o => o.Customers)
            .HasForeignKey(c => c.OutletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## 7. Error Handling

### 7.1 Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Token missing/empty | Add `token` header |
| 404 Not Found | Resource doesn't exist | Check ID |
| 400 Bad Request | Invalid request body | Check required fields |
| 409 Conflict | Duplicate unique field | Use different code/SKU |

### 7.2 Validation

```csharp
public class CreateOutletRequest
{
    [Required(ErrorMessage = "Code is required")]
    [StringLength(20, ErrorMessage = "Code max 20 characters")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public OutletType Type { get; set; }
}
```

## 8. Deployment

### 8.1 Prerequisites

- .NET 8 Runtime
- SQL Server 2019+
- IIS / Kestrel / Docker

### 8.2 Environment Variables

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=...;Database=...;..."
```

### 8.3 Build & Publish

```bash
dotnet publish -c Release -o ./publish
```

## 9. Security Considerations

1. **Token Validation**: Token dari external WIT API harus ada untuk akses API
2. **HTTPS**: Selalu gunakan HTTPS di production
3. **Connection String**: Jangan hardcode credentials, gunakan environment variables
4. **Input Validation**: Semua input divalidasi sebelum disimpan ke database
5. **SQL Injection**: Menggunakan EF Core parameterized queries

## 10. Performance Optimization

1. **Indexing**: Index pada kolom yang sering di-query
2. **Async/Await**: Semua database operations async
3. **Projection**: Gunakan Select() untuk mengambil field yang diperlukan saja
4. **Pagination**: Implementasi pagination untuk list endpoints
5. **Caching**: Consider Redis untuk frequently accessed data

---

*Document Version: 1.0*
*Last Updated: January 2026*
