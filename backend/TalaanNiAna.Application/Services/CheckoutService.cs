public class CheckoutService
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CheckoutResultDto> ProcessSaleAsync(CheckoutDto dto)
    {
        if (!Enum.TryParse<PaymentType>(dto.PaymentType, ignoreCase: true, out var paymentType))
            throw new ArgumentException($"Invalid payment type: {dto.PaymentType}");

        if (paymentType == PaymentType.Credit && dto.CustomerId is null)
            throw new InvalidOperationException("CustomerId is required for Credit (Utang) sales.");

        if (!dto.Items.Any())
            throw new InvalidOperationException("Cannot checkout an empty cart.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var transaction = new Transaction
            {
                TransactionDate = DateTime.UtcNow,
                PaymentType = paymentType,
                CustomerId = dto.CustomerId,
                TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice)
            };

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                    ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for {product.Name}. Available: {product.StockQuantity}, requested: {item.Quantity}.");

                product.StockQuantity -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);

                transaction.Items.Add(new TransactionItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            await _unitOfWork.Transactions.CreateAsync(transaction);

            var customerExists = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId!.Value) is not null;
            if (!customerExists)
                throw new InvalidOperationException("Customer not found.");

            await _unitOfWork.Customers.UpdateCreditAsync(dto.CustomerId!.Value, transaction.TotalAmount);

            await _unitOfWork.CommitAsync();

            return new CheckoutResultDto(transaction.Id, transaction.TotalAmount, transaction.TransactionDate);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}