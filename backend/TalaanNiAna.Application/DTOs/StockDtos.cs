public record StockMovementDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    string Reason,
    int BalanceAfter,
    int? TransactionId,
    string? Note,
    DateTime OccurredAt);

/// <summary>
/// A movement the owner records by hand. Quantity is signed, so a delivery and
/// a spoilage use the same shape rather than two endpoints that could disagree.
/// </summary>
public record RecordStockDto(
    int ProductId,
    int Quantity,
    string Reason,
    string? Note = null);

public record ShrinkageLineDto(
    string ProductName,
    int Quantity,
    string Reason,
    decimal CostLost,
    string? Note,
    DateTime OccurredAt);

public record ShrinkageReportDto(
    DateOnly From,
    DateOnly To,
    int TotalPieces,
    decimal TotalCost,
    IEnumerable<ShrinkageLineDto> Lines);
