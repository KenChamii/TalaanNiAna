using FluentAssertions;
using TalaanNiAna.Application.Common;
using Xunit;

namespace TalaanNiAna.Tests.Application;

public class StoreClockTests
{
    private const string Manila = "Asia/Manila";

    [Fact]
    public void Today_starts_at_local_midnight_not_utc_midnight()
    {
        // 14:00 UTC is 22:00 in Manila on the same day.
        var clock = new StoreClock(Manila, Utc(2026, 10, 17, 06, 00));

        var today = clock.Today();

        today.FromUtc.Should().Be(Utc(2026, 10, 16, 16, 00));
        today.ToUtc.Should().Be(Utc(2026, 10, 17, 16, 00));
    }

    [Fact]
    public void Today_includes_a_sale_made_just_after_local_midnight()
    {
        // 17 Oct 01:00 in Manila is still 16 Oct in UTC, which is the bug
        // this replaces: a UTC day would have excluded it.
        var saleUtc = Utc(2026, 10, 16, 17, 00);
        var clock = new StoreClock(Manila, Utc(2026, 10, 17, 06, 00));

        var today = clock.Today();

        saleUtc.Should().BeOnOrAfter(today.FromUtc).And.BeBefore(today.ToUtc);
    }

    [Fact]
    public void Today_excludes_a_sale_from_the_previous_local_day()
    {
        // 16 Oct 23:00 Manila belongs to the 16th, not the 17th.
        var saleUtc = Utc(2026, 10, 16, 15, 00);
        var clock = new StoreClock(Manila, Utc(2026, 10, 17, 06, 00));

        saleUtc.Should().BeBefore(clock.Today().FromUtc);
    }

    [Fact]
    public void Today_rolls_over_at_local_midnight_not_eight_hours_late()
    {
        // 16:30 UTC is 00:30 on the 18th in Manila, so "today" is the 18th.
        var clock = new StoreClock(Manila, Utc(2026, 10, 17, 16, 30));

        clock.Today().FromUtc.Should().Be(Utc(2026, 10, 17, 16, 00));
    }

    [Fact]
    public void Month_to_date_starts_at_the_local_first_of_the_month()
    {
        var clock = new StoreClock(Manila, Utc(2026, 10, 17, 06, 00));

        var month = clock.MonthToDate();

        month.FromUtc.Should().Be(Utc(2026, 09, 30, 16, 00));
        month.ToUtc.Should().Be(Utc(2026, 10, 31, 16, 00));
    }

    [Fact]
    public void Manila_is_eight_hours_ahead()
    {
        new StoreClock(Manila).UtcOffsetHours.Should().Be(8);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Falls_back_to_Manila_when_no_zone_is_set(string? timeZoneId)
    {
        new StoreClock(timeZoneId).UtcOffsetHours.Should().Be(8);
    }

    [Fact]
    public void Falls_back_rather_than_throwing_on_an_unknown_zone()
    {
        // A bad settings value must not take the dashboard down.
        var act = () => new StoreClock("Not/AZone").Today();

        act.Should().NotThrow();
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
        => new(year, month, day, hour, minute, 0, DateTimeKind.Utc);
}
