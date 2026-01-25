# Warehouse & POS System API

Sistem API untuk manajemen gudang (Warehouse Management System) dan Point of Sale (POS) yang terintegrasi dengan external authentication API WIT.

## Daftar Isi

- [Fitur](#fitur)
- [Tech Stack](#tech-stack)
- [Persyaratan](#persyaratan)
- [Instalasi](#instalasi)
- [Konfigurasi](#konfigurasi)
- [Menjalankan Aplikasi](#menjalankan-aplikasi)
- [Autentikasi](#autentikasi)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Testing](#testing)

## Fitur

- **Master Data Management**: Outlet, Customer, Supplier, Product, Product Variant
- **Inventory Management**: Stock tracking, Stock Opname, Stock Log
- **Transaction Processing**: Orders (POS), Purchases, Returns
- **Multi-tier Pricing**: Customer tier-based pricing
- **External Authentication**: Terintegrasi dengan WIT API untuk autentikasi token

## Tech Stack

- **.NET 8** - Framework backend
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **Swagger/OpenAPI** - API Documentation
- **External WIT API** - Authentication provider

## Persyaratan

- .NET 8 SDK
- SQL Server 2019+ atau SQL Server Express
- Visual Studio 2022 / VS Code
- Koneksi internet (untuk autentikasi ke WIT API)

## Instalasi

1. **Clone repository**
   ```bash
   git clone https://github.com/[username]/WarehousePOS.git
   cd WarehousePOS
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string** di `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=WarehousePOS;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

4. **Jalankan migrasi database**
   ```bash
   cd WarehousePOS
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Konfigurasi

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WarehousePOS;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "ExternalAuth": {
    "BaseUrl": "https://api-wms.wit.co.id"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Menjalankan Aplikasi

```bash
cd WarehousePOS
dotnet run
```

Aplikasi akan berjalan di:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Swagger UI tersedia di: `https://localhost:5001/swagger`

## Autentikasi

Sistem menggunakan token dari external WIT API untuk autentikasi.

### Flow Autentikasi

```
┌─────────────┐     1. Generate Token      ┌─────────────┐
│   Client    │ ─────────────────────────► │   WIT API   │
│             │ ◄───────────────────────── │             │
└─────────────┘     2. Return Token        └─────────────┘
       │
       │ 3. Request dengan Token
       ▼
┌─────────────┐
│  Local API  │ ──► 4. Validasi token ada
│ (this app)  │ ──► 5. Proses CRUD ke DB lokal
└─────────────┘
```

### Langkah Mendapatkan Token

1. **Generate token dari WIT API**:
   ```http
   POST https://api-wms.wit.co.id/GenerateToken
   Content-Type: application/json

   {
     "app_name": "wms",
     "app_key": "wms-app-key",
     "device_id": "your-device-id",
     "device_type": "00031310",
     "fcm_token": "",
     "ip_address": "0.0.0.0"
   }
   ```

2. **Gunakan token untuk akses API lokal**:
   ```http
   GET /api/Outlets
   token: {your_token_from_WIT}
   ```

   Atau menggunakan Authorization header:
   ```http
   GET /api/Outlets
   Authorization: Bearer {your_token_from_WIT}
   ```

### Endpoint Auth Helper (Opsional)

API ini juga menyediakan endpoint helper untuk generate token:

```http
POST /api/Auth/generate-token
```

## API Endpoints

### Authentication
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| POST | `/api/Auth/generate-token` | Generate token dari WIT API |
| POST | `/api/Auth/login` | Login dengan credentials |
| POST | `/api/Auth/full-auth` | Generate token + Login |

### Master Data - Outlet
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/Outlets` | Get all outlets |
| GET | `/api/Outlets/{id}` | Get outlet by ID |
| POST | `/api/Outlets` | Create outlet |
| PUT | `/api/Outlets/{id}` | Update outlet |
| DELETE | `/api/Outlets/{id}` | Delete outlet |

### Master Data - Customer
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/Customers` | Get all customers |
| GET | `/api/Customers/{id}` | Get customer by ID |
| POST | `/api/Customers` | Create customer |
| PUT | `/api/Customers/{id}` | Update customer |
| DELETE | `/api/Customers/{id}` | Delete customer |

### Master Data - Customer Tier
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/CustomerTiers` | Get all tiers |
| GET | `/api/CustomerTiers/{id}` | Get tier by ID |
| POST | `/api/CustomerTiers` | Create tier |
| PUT | `/api/CustomerTiers/{id}` | Update tier |
| DELETE | `/api/CustomerTiers/{id}` | Delete tier |

### Master Data - Supplier
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/Suppliers` | Get all suppliers |
| GET | `/api/Suppliers/{id}` | Get supplier by ID |
| POST | `/api/Suppliers` | Create supplier |
| PUT | `/api/Suppliers/{id}` | Update supplier |
| DELETE | `/api/Suppliers/{id}` | Delete supplier |

### Product Management
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/ProductTypes` | Get all product types |
| POST | `/api/ProductTypes` | Create product type |
| GET | `/api/Products` | Get all products |
| POST | `/api/Products` | Create product |
| GET | `/api/ProductVariants` | Get all variants |
| POST | `/api/ProductVariants` | Create variant |

### Inventory
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/Stocks` | Get all stocks |
| POST | `/api/Stocks` | Create/update stock |
| GET | `/api/StockOpnames` | Get all stock opnames |
| POST | `/api/StockOpnames` | Create stock opname |

### Transactions
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/Orders` | Get all orders |
| POST | `/api/Orders` | Create order |
| GET | `/api/Purchases` | Get all purchases |
| POST | `/api/Purchases` | Create purchase |
| GET | `/api/OrderReturns` | Get all returns |
| POST | `/api/OrderReturns` | Create return |

### Reference Data
| Method | Endpoint | Deskripsi |
|--------|----------|-----------|
| GET | `/api/OrderTypes` | Get order types |
| GET | `/api/PaymentMethods` | Get payment methods |

## Database Schema

### Entity Relationship

```
Outlet (1) ──────────────── (N) Customer
   │                              │
   │                              └── CustomerTier (N:1)
   │
   ├── (N) ProductStock ──── (1) ProductVariant ──── (1) Product
   │         │                                            │
   │         └── StockLocation (N:1)                      └── ProductType (N:1)
   │
   ├── (N) Order ──── (N) OrderDetail ──── (1) ProductVariant
   │         │
   │         ├── OrderPayment (N)
   │         └── OrderReturn (N) ──── ReturnDetail (N)
   │
   └── (N) Purchase ──── (N) PurchaseDetail
             │
             └── Supplier (N:1)
```

### Tabel Utama

| Tabel | Deskripsi |
|-------|-----------|
| `outlets` | Data outlet/toko |
| `customers` | Data pelanggan |
| `customer_tiers` | Tier pelanggan untuk pricing |
| `suppliers` | Data supplier |
| `product_types` | Kategori produk |
| `products` | Master produk |
| `product_variants` | Varian produk (SKU) |
| `product_stocks` | Stok per lokasi |
| `orders` | Transaksi penjualan |
| `order_details` | Detail item penjualan |
| `purchases` | Transaksi pembelian |
| `purchase_details` | Detail item pembelian |
| `stock_opnames` | Stock opname/adjustment |
| `order_returns` | Retur penjualan |

## Testing

### Menggunakan Postman

Import collection dari file `WarehousePOS_Postman_Collection.json`

### Contoh Request

**1. Generate Token**
```bash
curl -X POST https://api-wms.wit.co.id/GenerateToken \
  -H "Content-Type: application/json" \
  -d '{
    "app_name": "wms",
    "app_key": "wms-app-key",
    "device_id": "test-device",
    "device_type": "00031310",
    "fcm_token": "",
    "ip_address": "0.0.0.0"
  }'
```

**2. Get Outlets (dengan token)**
```bash
curl -X GET https://localhost:5001/api/Outlets \
  -H "token: {your_token}"
```

**3. Create Outlet**
```bash
curl -X POST https://localhost:5001/api/Outlets \
  -H "token: {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "OUT001",
    "name": "Outlet Pusat",
    "type": "Warehouse",
    "address": "Jl. Sudirman No. 1",
    "city": "Jakarta",
    "phone": "021-1234567"
  }'
```

## Struktur Project

```
WarehousePOS/
├── Controllers/           # API Controllers
│   ├── AuthController.cs
│   ├── OutletsController.cs
│   ├── CustomersController.cs
│   └── ...
├── Data/
│   └── AppDbContext.cs    # EF Core DbContext
├── DTOs/                  # Data Transfer Objects
│   ├── AuthDTOs.cs
│   ├── OutletDTOs.cs
│   └── ...
├── Models/                # Entity Models
│   ├── BaseEntity.cs
│   ├── Outlet.cs
│   ├── Customer.cs
│   └── ...
├── Services/              # Business Services
│   ├── AuthService.cs
│   └── ExternalTokenAuthHandler.cs
├── Program.cs             # Application entry point
├── appsettings.json       # Configuration
└── WarehousePOS.csproj    # Project file
```

## Author

Technical Test - WIT Indonesia

## License

This project is created for technical assessment purposes.
