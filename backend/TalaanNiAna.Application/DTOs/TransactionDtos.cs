public record TransactionItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal Subtotal);

public record TransactionDto(
    int Id, DateTime TransactionDate, decimal TotalAmount, string PaymentType,
    string? CustomerName, IEnumerable<TransactionItemDto> Items);