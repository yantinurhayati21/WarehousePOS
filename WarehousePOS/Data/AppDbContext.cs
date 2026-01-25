using Microsoft.EntityFrameworkCore;
using WarehousePOS.Models;

namespace WarehousePOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<CustomerTier> CustomerTiers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<PriceTier> PriceTiers { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<StockLocation> StockLocations { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<StockLog> StockLogs { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<OrderReturn> OrderReturns { get; set; }
        public DbSet<ReturnDetail> ReturnDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<StockOpname> StockOpnames { get; set; }
        public DbSet<SoDetail> SoDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======= OUTLET =======
            modelBuilder.Entity<Outlet>(entity =>
            {
                entity.HasIndex(e => e.OutletCode).IsUnique();
                entity.Property(e => e.OutletType).HasConversion<string>();
            });

            // ======= CUSTOMER =======
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.CustomerCode).IsUnique();
                entity.HasIndex(e => e.TierId);

                entity.HasOne(c => c.Tier)
                      .WithMany(t => t.Customers)
                      .HasForeignKey(c => c.TierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= SUPPLIER =======
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(e => e.SupplierCode).IsUnique();
            });

            // ======= PRODUCT =======
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.ProductCode).IsUnique();
                entity.HasIndex(e => e.TypeId);
                entity.Property(e => e.BaseUnit).HasConversion<string>();

                entity.HasOne(p => p.Type)
                      .WithMany(t => t.Products)
                      .HasForeignKey(p => p.TypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= PRODUCT VARIANT =======
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.VariantCode);
                entity.HasIndex(e => e.Barcode);

                entity.HasOne(v => v.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(v => v.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= PRODUCT PRICE =======
            modelBuilder.Entity<ProductPrice>(entity =>
            {
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.VariantId);
                entity.HasIndex(e => e.PriceTierId);
                entity.HasIndex(e => e.OutletId);

                entity.HasOne(pp => pp.Product)
                      .WithMany()
                      .HasForeignKey(pp => pp.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pp => pp.Variant)
                      .WithMany()
                      .HasForeignKey(pp => pp.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= PRODUCT STOCK =======
            modelBuilder.Entity<ProductStock>(entity =>
            {
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.VariantId);
                entity.HasIndex(e => e.OutletId);

                entity.HasOne(ps => ps.Product)
                      .WithMany()
                      .HasForeignKey(ps => ps.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ps => ps.Variant)
                      .WithMany()
                      .HasForeignKey(ps => ps.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ps => ps.Outlet)
                      .WithMany()
                      .HasForeignKey(ps => ps.OutletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= STOCK LOG =======
            modelBuilder.Entity<StockLog>(entity =>
            {
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.OutletId);
                entity.HasIndex(e => e.TransactionType);
                entity.HasIndex(e => e.ReferenceId);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.TransactionType).HasConversion<string>();
                entity.Property(e => e.ReferenceTable).HasConversion<string>();

                entity.HasOne(sl => sl.Product)
                      .WithMany()
                      .HasForeignKey(sl => sl.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= ORDER =======
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.OrderDate);
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.OutletId);
                entity.HasIndex(e => e.OrderStatus);
                entity.Property(e => e.OrderStatus).HasConversion<string>();

                entity.HasOne(o => o.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Outlet)
                      .WithMany(outl => outl.Orders)
                      .HasForeignKey(o => o.OutletId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.CustomerTier)
                      .WithMany(t => t.Orders)
                      .HasForeignKey(o => o.CustomerTierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= ORDER DETAIL =======
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ProductId);

                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                      .WithMany()
                      .HasForeignKey(od => od.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(od => od.Variant)
                      .WithMany()
                      .HasForeignKey(od => od.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= ORDER PAYMENT =======
            modelBuilder.Entity<OrderPayment>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.PaymentMethodId);
                entity.HasIndex(e => e.PaymentDate);
                entity.Property(e => e.PaymentStatus).HasConversion<string>();

                entity.HasOne(op => op.Order)
                      .WithMany(o => o.OrderPayments)
                      .HasForeignKey(op => op.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(op => op.PaymentMethod)
                      .WithMany()
                      .HasForeignKey(op => op.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= ORDER RETURN =======
            modelBuilder.Entity<OrderReturn>(entity =>
            {
                entity.HasIndex(e => e.ReturnNumber).IsUnique();
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ReturnDate);
                entity.Property(e => e.ReturnStatus).HasConversion<string>();

                entity.HasOne(or => or.Order)
                      .WithMany(o => o.OrderReturns)
                      .HasForeignKey(or => or.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======= RETURN DETAIL =======
            modelBuilder.Entity<ReturnDetail>(entity =>
            {
                entity.HasIndex(e => e.ReturnId);
                entity.HasIndex(e => e.OrderDetailId);

                entity.HasOne(rd => rd.OrderReturn)
                      .WithMany(or => or.ReturnDetails)
                      .HasForeignKey(rd => rd.ReturnId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rd => rd.OrderDetail)
                      .WithMany()
                      .HasForeignKey(rd => rd.OrderDetailId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rd => rd.Product)
                      .WithMany()
                      .HasForeignKey(rd => rd.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rd => rd.Variant)
                      .WithMany()
                      .HasForeignKey(rd => rd.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= PURCHASE =======
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasIndex(e => e.PurchaseNumber).IsUnique();
                entity.HasIndex(e => e.PurchaseDate);
                entity.HasIndex(e => e.OutletId);
                entity.HasIndex(e => e.SupplierId);
                entity.Property(e => e.PurchaseStatus).HasConversion<string>();

                entity.HasOne(p => p.Outlet)
                      .WithMany(o => o.Purchases)
                      .HasForeignKey(p => p.OutletId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Supplier)
                      .WithMany(s => s.Purchases)
                      .HasForeignKey(p => p.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= PURCHASE DETAIL =======
            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.HasIndex(e => e.PurchaseId);
                entity.HasIndex(e => e.ProductId);

                entity.HasOne(pd => pd.Purchase)
                      .WithMany(p => p.PurchaseDetails)
                      .HasForeignKey(pd => pd.PurchaseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pd => pd.Product)
                      .WithMany()
                      .HasForeignKey(pd => pd.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pd => pd.Variant)
                      .WithMany()
                      .HasForeignKey(pd => pd.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= STOCK OPNAME =======
            modelBuilder.Entity<StockOpname>(entity =>
            {
                entity.HasIndex(e => e.SoNumber).IsUnique();
                entity.HasIndex(e => e.OutletId);
                entity.HasIndex(e => e.SoDate);
                entity.Property(e => e.SoStatus).HasConversion<string>();

                entity.HasOne(so => so.Outlet)
                      .WithMany(o => o.StockOpnames)
                      .HasForeignKey(so => so.OutletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ======= SO DETAIL =======
            modelBuilder.Entity<SoDetail>(entity =>
            {
                entity.HasIndex(e => e.SoId);
                entity.HasIndex(e => e.ProductId);

                entity.HasOne(sd => sd.StockOpname)
                      .WithMany(so => so.SoDetails)
                      .HasForeignKey(sd => sd.SoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sd => sd.Product)
                      .WithMany()
                      .HasForeignKey(sd => sd.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sd => sd.Variant)
                      .WithMany()
                      .HasForeignKey(sd => sd.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
