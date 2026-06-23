public record CustomerDto(
    int Id, string FullName, string? ContactNumber,
    decimal TotalCredit, DateTime LastActiveAt);

public record CreateCustomerDto(string FullName, string? ContactNumber);

public record CustomerDetailDto(
    int Id, string FullName, string? ContactNumber, decimal TotalCredit,
    DateTime LastActiveAt, IEnumerable<CustomerTransactionDto> Transactions);

public record CustomerTransactionDto(
    int Id, DateTime TransactionDate, decimal TotalAmount,
    string PaymentType, IEnumerable<string> ItemSummaries);