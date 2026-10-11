namespace TalaanNiAna.Application.Common.Exceptions;

/// <summary>The request is valid but the current state rejects it. Maps to 409.</summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message, string code = "conflict") : base(message)
    {
        Code = code;
    }

    public override string Code { get; }

    public static ConflictException InsufficientStock(string productName, int available, int requested) =>
        new($"Kulang ang stock ng {productName}. Natitira {available}, hinihingi {requested}.",
            "insufficient_stock");

    public static ConflictException PriceChanged(string productName) =>
        new($"Nagbago ang presyo ng {productName}. Pakiscan ulit.", "price_changed");

    public static ConflictException StockChanged() =>
        new("Nabago ang stock habang nagche-checkout. Pakisubukan ulit.", "stock_changed");
}
