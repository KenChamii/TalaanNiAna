using TalaanNiAna.Application.Common.Exceptions;

/// <summary>
/// Breaking a pack open into tingi. A sack of rice becomes forty cups, a
/// twelve-pack of shampoo becomes twelve sachets. The stock on both sides
/// moves in one transaction, and the ledger records both halves, so the pack
/// that disappeared is accounted for by the sachets that appeared.
/// </summary>
public class RepackService
{
    private readonly IUnitOfWork _unitOfWork;

    public RepackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RepackResultDto> RepackAsync(RepackDto dto)
    {
        if (dto.ParentQuantity < 1)
            throw new RequestValidationException(nameof(dto.ParentQuantity), "Ilan ang bubuksan?");

        var tingi = await _unitOfWork.Products.GetByIdAsync(dto.TingiProductId)
            ?? throw new NotFoundException("Produkto", dto.TingiProductId);

        if (!tingi.IsTingi)
            throw new RequestValidationException(nameof(dto.TingiProductId),
                $"Walang pinagmulang pakete ang {tingi.Name}.");

        var parent = await _unitOfWork.Products.GetByIdAsync(tingi.ParentProductId!.Value)
            ?? throw new NotFoundException("Produkto", tingi.ParentProductId!.Value);

        if (parent.StockQuantity < dto.ParentQuantity)
            throw ConflictException.InsufficientStock(parent.Name, parent.StockQuantity, dto.ParentQuantity);

        var produced = dto.ParentQuantity * tingi.UnitsPerParent!.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            parent.StockQuantity -= dto.ParentQuantity;
            tingi.StockQuantity += produced;

            // The cost per piece is the pack's cost split across what it yields.
            // Recomputed on every repack, because the pack's own cost may have
            // changed since the last one.
            if (parent.CostPrice > 0)
                tingi.CostPrice = Math.Round(parent.CostPrice / tingi.UnitsPerParent.Value, 2);

            parent.UpdatedAt = DateTime.UtcNow;
            tingi.UpdatedAt = DateTime.UtcNow;

            var occurredAt = DateTime.UtcNow;
            var note = $"{dto.ParentQuantity} x {parent.Name}";

            _unitOfWork.StockMovements.Add(new StockMovement
            {
                ProductId = parent.Id,
                Product = parent,
                Quantity = -dto.ParentQuantity,
                Reason = StockReason.RepackOut,
                BalanceAfter = parent.StockQuantity,
                Note = $"Tiningi bilang {tingi.Name}",
                OccurredAt = occurredAt
            });

            _unitOfWork.StockMovements.Add(new StockMovement
            {
                ProductId = tingi.Id,
                Product = tingi,
                Quantity = produced,
                Reason = StockReason.RepackIn,
                BalanceAfter = tingi.StockQuantity,
                Note = $"Galing sa {note}",
                OccurredAt = occurredAt
            });

            await _unitOfWork.CommitAsync();

            return new RepackResultDto(
                ParentName: parent.Name,
                ParentRemaining: parent.StockQuantity,
                TingiName: tingi.Name,
                TingiProduced: produced,
                TingiRemaining: tingi.StockQuantity,
                UnitCost: tingi.CostPrice);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
