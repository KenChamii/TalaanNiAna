public class TransactionService
{
    private readonly ITransactionRepository _repo;

    public TransactionService(ITransactionRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(
        DateTime? dateFrom, DateTime? dateTo, PaymentType? paymentType, int page = 1, int pageSize = 10)
    {
        var transactions = await _repo.GetAllAsync(dateFrom, dateTo, paymentType, page, pageSize);
        var total = await _repo.GetTotalCountAsync(dateFrom, dateTo, paymentType);

        return new PagedResult<TransactionDto>
        {
            Items = transactions.Select(ToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    private static TransactionDto ToDto(Transaction t) => new(
        t.Id, t.TransactionDate, t.TotalAmount, t.PaymentType.ToString(),
        t.Customer?.FullName,
        t.Items.Select(i => new TransactionItemDto(i.Product.Name, i.Quantity, i.UnitPrice, i.Subtotal)));
}