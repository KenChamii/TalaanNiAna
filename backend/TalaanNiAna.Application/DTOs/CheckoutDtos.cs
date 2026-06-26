public record CheckoutItemDto(int ProductId, int Quantity, decimal UnitPrice);

public record CheckoutDto(
    string PaymentType,        // "Cash" or "Credit" — matches the Angular payload in 5.14
    int? CustomerId,
    IEnumerable<CheckoutItemDto> Items);

public record CheckoutResultDto(int TransactionId, decimal TotalAmount, DateTime TransactionDate);