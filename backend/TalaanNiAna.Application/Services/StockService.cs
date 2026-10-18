using TalaanNiAna.Application.Common;
using TalaanNiAna.Application.Common.Exceptions;

/// <summary>
/// Stock changes that are not sales. Every one of them writes a ledger row, so
/// the quantity on a product is always explainable by the movements behind it.
/// </summary>
public class StockService
{
    /// <summary>Reasons the owner may record by hand. Sales and repacks are written by their own flows.</summary>
    private static readonly StockReason[] Manual =
    [
        StockReason.Restock,
        StockReason.Adjustment,
        StockReason.Spoilage,
        StockReason.Return
    ];

    private readonly IUnitOfWork _unitOfWork;
    private readonly IStoreRepository _stores;

    public StockService(IUnitOfWork unitOfWork, IStoreRepository stores)
    {
        _unitOfWork = unitOfWork;
        _stores = stores;
    }

    public async Task<StockMovementDto> RecordAsync(RecordStockDto dto)
    {
        var reason = ParseReason(dto.Reason);

        if (dto.Quantity == 0)
            throw new RequestValidationException(nameof(dto.Quantity), "Walang pagbabago sa stock.");

        // Spoilage that adds stock, or a delivery that removes it, is a typo
        // rather than an intention worth recording.
        if (reason is StockReason.Spoilage && dto.Quantity > 0)
            throw new RequestValidationException(nameof(dto.Quantity), "Dapat negatibo ang nasirang stock.");

        if (reason is StockReason.Restock or StockReason.Return && dto.Quantity < 0)
            throw new RequestValidationException(nameof(dto.Quantity), "Dapat positibo ang dagdag na stock.");

        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId)
            ?? throw new NotFoundException("Produkto", dto.ProductId);

        var balance = product.StockQuantity + dto.Quantity;

        if (balance < 0)
            throw ConflictException.InsufficientStock(product.Name, product.StockQuantity, -dto.Quantity);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            product.StockQuantity = balance;
            product.UpdatedAt = DateTime.UtcNow;

            var movement = new StockMovement
            {
                ProductId = product.Id,
                // Attached as well as referenced, so a caller reading the result
                // back does not need a second round trip for the name and cost.
                Product = product,
                Quantity = dto.Quantity,
                Reason = reason,
                BalanceAfter = balance,
                Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim()
            };

            _unitOfWork.StockMovements.Add(movement);

            await _unitOfWork.CommitAsync();

            return ToDto(movement, product.Name);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<StockMovementDto>> GetHistoryAsync(int productId, int count = 50)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException("Produkto", productId);

        var movements = await _unitOfWork.StockMovements.GetForProductAsync(productId, count);

        return movements.Select(m => ToDto(m, product.Name));
    }

    /// <summary>
    /// What left the shelf without being sold. Valued at the product's cost,
    /// because that is what the owner actually paid for the loss.
    /// </summary>
    public async Task<ShrinkageReportDto> GetShrinkageAsync(DateOnly? from = null, DateOnly? to = null)
    {
        var store = await _stores.GetCurrentAsync();
        var clock = new StoreClock(store.TimeZoneId);

        var last = to ?? clock.LocalToday;
        var first = from ?? last.AddDays(-29);

        if (first > last)
            throw new RequestValidationException(nameof(from), "Nauuna ang huling petsa sa simula.");

        var range = new DateRange(clock.Day(first).FromUtc, clock.Day(last).ToUtc);
        var movements = (await _unitOfWork.StockMovements.GetShrinkageAsync(range)).ToList();

        var lines = movements.Select(m => new ShrinkageLineDto(
            m.Product.Name,
            m.Quantity,
            m.Reason.ToString(),
            // Quantity is negative, so negate to express the loss as a positive cost.
            Math.Round(-m.Quantity * m.Product.CostPrice, 2),
            m.Note,
            m.OccurredAt)).ToList();

        return new ShrinkageReportDto(
            From: first,
            To: last,
            TotalPieces: lines.Sum(l => -l.Quantity),
            TotalCost: lines.Sum(l => l.CostLost),
            Lines: lines);
    }

    private static StockMovementDto ToDto(StockMovement m, string productName) => new(
        m.Id, m.ProductId, productName, m.Quantity, m.Reason.ToString(),
        m.BalanceAfter, m.TransactionId, m.Note, m.OccurredAt);

    private static StockReason ParseReason(string? value)
    {
        if (!Enum.TryParse<StockReason>(value, ignoreCase: true, out var reason) || !Manual.Contains(reason))
            throw new RequestValidationException(nameof(RecordStockDto.Reason),
                $"Hindi pwedeng itala nang manu-mano: {value}.");

        return reason;
    }
}
