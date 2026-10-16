namespace TalaanNiAna.Application.Common;

/// <summary>
/// Resolves "today" and "this month" in the store's own timezone and returns
/// them as UTC windows. A Manila store closing at 9pm must not have its last
/// hours counted against the next day, which UTC midnight would do.
/// </summary>
public sealed class StoreClock
{
    public const string DefaultTimeZoneId = "Asia/Manila";

    /// <summary>Windows ids, tried when the IANA id is unavailable.</summary>
    private static readonly Dictionary<string, string> WindowsFallbacks = new()
    {
        ["Asia/Manila"] = "Singapore Standard Time"
    };

    private readonly TimeZoneInfo _zone;
    private readonly DateTime _utcNow;

    public StoreClock(string? timeZoneId, DateTime? utcNow = null)
    {
        _zone = Resolve(timeZoneId);
        _utcNow = utcNow ?? DateTime.UtcNow;
    }

    public DateTime LocalNow => TimeZoneInfo.ConvertTimeFromUtc(_utcNow, _zone);

    /// <summary>
    /// Whole hours the store is ahead of UTC. Used to group by local date in SQL,
    /// which is exact only where the zone has no DST. The Philippines has none.
    /// </summary>
    public int UtcOffsetHours => (int)_zone.GetUtcOffset(_utcNow).TotalHours;

    /// <summary>True when this id resolves here. Used to validate the setting.</summary>
    public static bool CanResolve(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId)) return false;

        return TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _)
            || (WindowsFallbacks.TryGetValue(timeZoneId, out var windowsId)
                && TimeZoneInfo.TryFindSystemTimeZoneById(windowsId, out _));
    }

    public DateRange Today()
    {
        var start = LocalNow.Date;
        return Range(start, start.AddDays(1));
    }

    public DateRange MonthToDate()
    {
        var local = LocalNow;
        var start = new DateTime(local.Year, local.Month, 1);
        return Range(start, start.AddMonths(1));
    }

    private DateRange Range(DateTime localFrom, DateTime localTo)
        => new(ToUtc(localFrom), ToUtc(localTo));

    private DateTime ToUtc(DateTime local)
        => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), _zone);

    private static TimeZoneInfo Resolve(string? timeZoneId)
    {
        var id = string.IsNullOrWhiteSpace(timeZoneId) ? DefaultTimeZoneId : timeZoneId;

        if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone))
            return zone;

        if (WindowsFallbacks.TryGetValue(id, out var windowsId) &&
            TimeZoneInfo.TryFindSystemTimeZoneById(windowsId, out var fallback))
            return fallback;

        // Never fail a dashboard over a bad id; the default beats UTC for this store.
        return TimeZoneInfo.CreateCustomTimeZone(id, TimeSpan.FromHours(8), id, id);
    }
}
