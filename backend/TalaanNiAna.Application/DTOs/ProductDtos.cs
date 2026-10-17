public record ProductDto(
    int Id,
    string Name,
    string Category,
    string? Barcode,
    decimal Price,
    decimal CostPrice,
    decimal UnitMargin,
    decimal MarginPercent,
    int StockQuantity,
    int LowStockThreshold,
    bool IsLowStock);

public record CreateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    decimal CostPrice = 0,
    string? Barcode = null,           // null for loose items with nothing to scan
    int? LowStockThreshold = null);   // null falls back to the store's default

public record UpdateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    decimal? CostPrice = null,        // null keeps the product's current cost
    string? Barcode = null,           // empty string clears it, null keeps it
    int? LowStockThreshold = null);   // null keeps the product's current value
