using TalaanNiAna.Application.Common.Exceptions;

public class CheckoutService
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CheckoutResultDto> ProcessSaleAsync(CheckoutDto dto)
    {
        var paymentType = ParsePaymentType(dto.PaymentType);
        var items = dto.Items?.ToList() ?? [];

        if (items.Count == 0)
            throw new RequestValidationException(nameof(dto.Items), "Walang laman ang kariton.");

        if (paymentType == PaymentType.Credit && dto.CustomerId is null)
            throw new RequestValidationException(nameof(dto.CustomerId), "Piliin kung sino ang uutang.");

        // A retried request must not sell twice.
        var replay = await _unitOfWork.Transactions.GetByIdempotencyKeyAsync(dto.IdempotencyKey);
        if (replay is not null)
            return new CheckoutResultDto(replay.Id, replay.TotalAmount, replay.TransactionDate, WasReplay: true);

        var products = await _unitOfWork.Products.GetByIdsAsync(items.Select(i => i.ProductId));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var transaction = BuildTransaction(dto, paymentType, items, products);

            await _unitOfWork.Transactions.CreateAsync(transaction);

            if (paymentType == PaymentType.Credit)
            {
                var customerId = dto.CustomerId!.Value;

                if (await _unitOfWork.Customers.GetByIdAsync(customerId) is null)
                    throw new NotFoundException("Suki", customerId);

                // Derived from the ledger, which now includes the transaction above.
                await _unitOfWork.Customers.RecalculateCreditAsync(customerId);
            }

            await _unitOfWork.CommitAsync();

            return new CheckoutResultDto(transaction.Id, transaction.TotalAmount, transaction.TransactionDate);
        }
        catch
        {
            // Includes DbUpdateConcurrencyException when another sale took the stock
            // first. The middleware maps that to 409; Application stays free of EF.
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static Transaction BuildTransaction(
        CheckoutDto dto,
        PaymentType paymentType,
        IReadOnlyList<CheckoutItemDto> items,
        IReadOnlyDictionary<int, Product> products)
    {
        var transaction = new Transaction
        {
            TransactionDate = DateTime.UtcNow,
            PaymentType = paymentType,
            CustomerId = dto.CustomerId,
            IdempotencyKey = dto.IdempotencyKey
        };

        foreach (var item in items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException("Produkto", item.ProductId);

            // The price is the server's, never the client's. Only the comparison uses theirs.
            if (item.UnitPrice != product.Price)
                throw ConflictException.PriceChanged(product.Name);

            if (product.StockQuantity < item.Quantity)
                throw ConflictException.InsufficientStock(product.Name, product.StockQuantity, item.Quantity);

            product.StockQuantity -= item.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            transaction.Items.Add(new TransactionItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        transaction.TotalAmount = transaction.Items.Sum(i => i.Quantity * i.UnitPrice);
        return transaction;
    }

    private static PaymentType ParsePaymentType(string? value)
    {
        if (!Enum.TryParse<PaymentType>(value, ignoreCase: true, out var paymentType))
            throw new RequestValidationException(nameof(CheckoutDto.PaymentType),
                $"Hindi kilalang uri ng bayad: {value}.");

        return paymentType;
    }
}
