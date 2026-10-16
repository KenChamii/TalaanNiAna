/// <summary>
/// A half-open UTC window, From inclusive and To exclusive.
/// Callers compute it from the store's timezone, so repositories never
/// have to assume a day starts at UTC midnight.
/// </summary>
public readonly record struct DateRange(DateTime FromUtc, DateTime ToUtc);
