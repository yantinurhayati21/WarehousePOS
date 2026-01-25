using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehousePOS.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_tiers",
                columns: table => new
                {
                    tier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tier_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tier_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    min_points = table.Column<int>(type: "int", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_tiers", x => x.tier_id);
                });

            migrationBuilder.CreateTable(
                name: "order_types",
                columns: table => new
                {
                    order_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_types", x => x.order_type_id);
                });

            migrationBuilder.CreateTable(
                name: "outlets",
                columns: table => new
                {
                    outlet_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    outlet_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    outlet_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    outlet_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outlets", x => x.outlet_id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    method_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    method_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.method_id);
                });

            migrationBuilder.CreateTable(
                name: "price_tiers",
                columns: table => new
                {
                    price_tier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tier_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tier_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_tiers", x => x.price_tier_id);
                });

            migrationBuilder.CreateTable(
                name: "product_types",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    type_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_types", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    supplier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplier_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    supplier_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.supplier_id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    customer_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tier_id = table.Column<int>(type: "int", nullable: true),
                    total_points = table.Column<int>(type: "int", nullable: false),
                    total_purchase = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK_customers_customer_tiers_tier_id",
                        column: x => x.tier_id,
                        principalTable: "customer_tiers",
                        principalColumn: "tier_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    location_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    location_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_locations", x => x.location_id);
                    table.ForeignKey(
                        name: "FK_stock_locations_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_opnames",
                columns: table => new
                {
                    so_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    so_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    so_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_variance = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    so_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    completed_by = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_opnames", x => x.so_id);
                    table.ForeignKey(
                        name: "FK_stock_opnames_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    product_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_id = table.Column<int>(type: "int", nullable: false),
                    base_unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    conversion_factor = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_products_product_types_type_id",
                        column: x => x.type_id,
                        principalTable: "product_types",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    purchase_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    purchase_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    purchase_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: false),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    purchase_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    received_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases", x => x.purchase_id);
                    table.ForeignKey(
                        name: "FK_purchases_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchases_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    order_type_id = table.Column<int>(type: "int", nullable: false),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    customer_tier_id = table.Column<int>(type: "int", nullable: true),
                    total_items = table.Column<int>(type: "int", nullable: false),
                    subtotal_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    shipping_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    order_status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_orders_customer_tiers_customer_tier_id",
                        column: x => x.customer_tier_id,
                        principalTable: "customer_tiers",
                        principalColumn: "tier_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_order_types_order_type_id",
                        column: x => x.order_type_id,
                        principalTable: "order_types",
                        principalColumn: "order_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    variant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    variant_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    weight = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variants", x => x.variant_id);
                    table.ForeignKey(
                        name: "FK_product_variants_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_payments",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_method_id = table.Column<int>(type: "int", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payment_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    reference_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    payment_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodMethodId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_payments", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_order_payments_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_payments_payment_methods_PaymentMethodMethodId",
                        column: x => x.PaymentMethodMethodId,
                        principalTable: "payment_methods",
                        principalColumn: "method_id");
                    table.ForeignKey(
                        name: "FK_order_payments_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "method_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_returns",
                columns: table => new
                {
                    return_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    return_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    return_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    return_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_refund_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    return_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_returns", x => x.return_id);
                    table.ForeignKey(
                        name: "FK_order_returns_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    order_detail_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantVariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.order_detail_id);
                    table.ForeignKey(
                        name: "FK_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_details_product_variants_ProductVariantVariantId",
                        column: x => x.ProductVariantVariantId,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_order_details_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_details_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_order_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_prices",
                columns: table => new
                {
                    price_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    price_tier_id = table.Column<int>(type: "int", nullable: false),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    min_quantity = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    ProductId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantVariantId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_prices", x => x.price_id);
                    table.ForeignKey(
                        name: "FK_product_prices_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_prices_price_tiers_price_tier_id",
                        column: x => x.price_tier_id,
                        principalTable: "price_tiers",
                        principalColumn: "price_tier_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_prices_product_variants_ProductVariantVariantId",
                        column: x => x.ProductVariantVariantId,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_product_prices_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_prices_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_product_prices_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_stocks",
                columns: table => new
                {
                    stock_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    min_stock = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    max_stock = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    last_updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutletId1 = table.Column<int>(type: "int", nullable: true),
                    ProductId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantVariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_stocks", x => x.stock_id);
                    table.ForeignKey(
                        name: "FK_product_stocks_outlets_OutletId1",
                        column: x => x.OutletId1,
                        principalTable: "outlets",
                        principalColumn: "outlet_id");
                    table.ForeignKey(
                        name: "FK_product_stocks_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_stocks_product_variants_ProductVariantVariantId",
                        column: x => x.ProductVariantVariantId,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_product_stocks_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_stocks_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_product_stocks_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_stocks_stock_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "stock_locations",
                        principalColumn: "location_id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_details",
                columns: table => new
                {
                    purchase_detail_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    purchase_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ProductId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantVariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_details", x => x.purchase_detail_id);
                    table.ForeignKey(
                        name: "FK_purchase_details_product_variants_ProductVariantVariantId",
                        column: x => x.ProductVariantVariantId,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_purchase_details_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_details_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_purchase_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_details_purchases_purchase_id",
                        column: x => x.purchase_id,
                        principalTable: "purchases",
                        principalColumn: "purchase_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "so_details",
                columns: table => new
                {
                    so_detail_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    so_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    system_quantity = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    physical_quantity = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    variance = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantVariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_so_details", x => x.so_detail_id);
                    table.ForeignKey(
                        name: "FK_so_details_product_variants_ProductVariantVariantId",
                        column: x => x.ProductVariantVariantId,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_so_details_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_so_details_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_so_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_so_details_stock_opnames_so_id",
                        column: x => x.so_id,
                        principalTable: "stock_opnames",
                        principalColumn: "so_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_logs",
                columns: table => new
                {
                    log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    outlet_id = table.Column<int>(type: "int", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: true),
                    transaction_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    reference_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    reference_table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity_before = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    quantity_change = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    quantity_after = table.Column<decimal>(type: "decimal(15,4)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    ProductId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_logs", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_stock_logs_outlets_outlet_id",
                        column: x => x.outlet_id,
                        principalTable: "outlets",
                        principalColumn: "outlet_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_logs_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                    table.ForeignKey(
                        name: "FK_stock_logs_products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_stock_logs_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_logs_stock_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "stock_locations",
                        principalColumn: "location_id");
                });

            migrationBuilder.CreateTable(
                name: "return_details",
                columns: table => new
                {
                    return_detail_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    return_id = table.Column<long>(type: "bigint", nullable: false),
                    order_detail_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: true),
                    return_quantity = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    refund_amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDetailId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_return_details", x => x.return_detail_id);
                    table.ForeignKey(
                        name: "FK_return_details_order_details_OrderDetailId1",
                        column: x => x.OrderDetailId1,
                        principalTable: "order_details",
                        principalColumn: "order_detail_id");
                    table.ForeignKey(
                        name: "FK_return_details_order_details_order_detail_id",
                        column: x => x.order_detail_id,
                        principalTable: "order_details",
                        principalColumn: "order_detail_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_return_details_order_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "order_returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_return_details_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_return_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_customer_code",
                table: "customers",
                column: "customer_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_tier_id",
                table: "customers",
                column: "tier_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_order_id",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_product_id",
                table: "order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_ProductId1",
                table: "order_details",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_ProductVariantVariantId",
                table: "order_details",
                column: "ProductVariantVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_variant_id",
                table: "order_details",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_order_id",
                table: "order_payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_payment_date",
                table: "order_payments",
                column: "payment_date");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_payment_method_id",
                table: "order_payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_PaymentMethodMethodId",
                table: "order_payments",
                column: "PaymentMethodMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_order_returns_order_id",
                table: "order_returns",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_returns_return_date",
                table: "order_returns",
                column: "return_date");

            migrationBuilder.CreateIndex(
                name: "IX_order_returns_return_number",
                table: "order_returns",
                column: "return_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_tier_id",
                table: "orders",
                column: "customer_tier_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_date",
                table: "orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_number",
                table: "orders",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_status",
                table: "orders",
                column: "order_status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_type_id",
                table: "orders",
                column: "order_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_outlet_id",
                table: "orders",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_outlets_outlet_code",
                table: "outlets",
                column: "outlet_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_outlet_id",
                table: "product_prices",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_price_tier_id",
                table: "product_prices",
                column: "price_tier_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_product_id",
                table: "product_prices",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_ProductId1",
                table: "product_prices",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_ProductVariantVariantId",
                table: "product_prices",
                column: "ProductVariantVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_variant_id",
                table: "product_prices",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_location_id",
                table: "product_stocks",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_outlet_id",
                table: "product_stocks",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_OutletId1",
                table: "product_stocks",
                column: "OutletId1");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_product_id",
                table: "product_stocks",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_ProductId1",
                table: "product_stocks",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_ProductVariantVariantId",
                table: "product_stocks",
                column: "ProductVariantVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_product_stocks_variant_id",
                table: "product_stocks",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_barcode",
                table: "product_variants",
                column: "barcode");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_product_id",
                table: "product_variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_variant_code",
                table: "product_variants",
                column: "variant_code");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_code",
                table: "products",
                column: "product_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_type_id",
                table: "products",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_product_id",
                table: "purchase_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_ProductId1",
                table: "purchase_details",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_ProductVariantVariantId",
                table: "purchase_details",
                column: "ProductVariantVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_purchase_id",
                table: "purchase_details",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_details_variant_id",
                table: "purchase_details",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_outlet_id",
                table: "purchases",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_purchase_date",
                table: "purchases",
                column: "purchase_date");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_purchase_number",
                table: "purchases",
                column: "purchase_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchases_supplier_id",
                table: "purchases",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_details_order_detail_id",
                table: "return_details",
                column: "order_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_details_OrderDetailId1",
                table: "return_details",
                column: "OrderDetailId1");

            migrationBuilder.CreateIndex(
                name: "IX_return_details_product_id",
                table: "return_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_details_return_id",
                table: "return_details",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_details_variant_id",
                table: "return_details",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_so_details_product_id",
                table: "so_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_so_details_ProductId1",
                table: "so_details",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_so_details_ProductVariantVariantId",
                table: "so_details",
                column: "ProductVariantVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_so_details_so_id",
                table: "so_details",
                column: "so_id");

            migrationBuilder.CreateIndex(
                name: "IX_so_details_variant_id",
                table: "so_details",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_outlet_id",
                table: "stock_locations",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_created_at",
                table: "stock_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_location_id",
                table: "stock_logs",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_outlet_id",
                table: "stock_logs",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_product_id",
                table: "stock_logs",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_ProductId1",
                table: "stock_logs",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_reference_id",
                table: "stock_logs",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_transaction_type",
                table: "stock_logs",
                column: "transaction_type");

            migrationBuilder.CreateIndex(
                name: "IX_stock_logs_variant_id",
                table: "stock_logs",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_opnames_outlet_id",
                table: "stock_opnames",
                column: "outlet_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_opnames_so_date",
                table: "stock_opnames",
                column: "so_date");

            migrationBuilder.CreateIndex(
                name: "IX_stock_opnames_so_number",
                table: "stock_opnames",
                column: "so_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_supplier_code",
                table: "suppliers",
                column: "supplier_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_payments");

            migrationBuilder.DropTable(
                name: "product_prices");

            migrationBuilder.DropTable(
                name: "product_stocks");

            migrationBuilder.DropTable(
                name: "purchase_details");

            migrationBuilder.DropTable(
                name: "return_details");

            migrationBuilder.DropTable(
                name: "so_details");

            migrationBuilder.DropTable(
                name: "stock_logs");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "price_tiers");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "order_returns");

            migrationBuilder.DropTable(
                name: "stock_opnames");

            migrationBuilder.DropTable(
                name: "stock_locations");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "order_types");

            migrationBuilder.DropTable(
                name: "outlets");

            migrationBuilder.DropTable(
                name: "product_types");

            migrationBuilder.DropTable(
                name: "customer_tiers");
        }
    }
}
