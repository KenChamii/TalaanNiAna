public record ProductDto(
    int Id,
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    bool IsLowStock);

public record CreateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    int LowStockThreshold = 10);

public record UpdateProductDto(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    int LowStockThreshold);