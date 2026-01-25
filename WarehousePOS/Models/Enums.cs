namespace WarehousePOS.Models
{
    public enum OutletType
    {
        Warehouse,
        Store,
        Branch
    }

    public enum BaseUnit
    {
        Kg,
        Gram,
        Pcs,
        Pack
    }

    public enum TransactionType
    {
        SO,
        PO,
        SALE,
        RETURN,
        ADJUSTMENT,
        TRANSFER
    }

    public enum ReferenceTable
    {
        Orders,
        Purchases,
        StockOpnames,
        Returns
    }

    public enum OrderStatus
    {
        Draft,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }

    public enum ReturnStatus
    {
        Requested,
        Approved,
        Rejected,
        Completed
    }

    public enum PurchaseStatus
    {
        Draft,
        Ordered,
        Received,
        Cancelled
    }

    public enum StockOpnameStatus
    {
        Draft,
        InProgress,
        Completed
    }
}
