/// <summary>
/// Breaks packs open. The tingi product is named rather than the parent,
/// because the owner's intent is "I need more sachets", and the pack to open
/// follows from that.
/// </summary>
public record RepackDto(int TingiProductId, int ParentQuantity = 1);

public record RepackResultDto(
    string ParentName,
    int ParentRemaining,
    string TingiName,
    int TingiProduced,
    int TingiRemaining,
    decimal UnitCost);

/// <summary>A product that can be a parent, for the form's picker.</summary>
public record RepackSourceDto(int Id, string Name, int StockQuantity, decimal CostPrice);
