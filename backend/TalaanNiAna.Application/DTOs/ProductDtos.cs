public record ProductDto(
    int Id,
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    int LowStockThreshold,
    bool IsLowStock);

public record CreateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    int? LowStockThreshold = null);   // null falls back to the store's default

public record UpdateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    int? LowStockThreshold = null);   // null keeps the product's current value
