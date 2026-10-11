public record CheckoutItemDto(int ProductId, int Quantity, decimal UnitPrice);

public record CheckoutDto(
    string PaymentType,        // "Cash" or "Credit"
    int? CustomerId,
    IEnumerable<CheckoutItemDto> Items,
    string IdempotencyKey);

public record CheckoutResultDto(
    int TransactionId,
    decimal TotalAmount,
    DateTime TransactionDate,
    bool WasReplay = false);
