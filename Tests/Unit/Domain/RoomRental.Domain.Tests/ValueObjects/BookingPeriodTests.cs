using RoomRental.Domain.Tests.TestData;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Domain.Tests.ValueObjects;

/// <summary>
/// Период бронирования: одна дата, целые часы, рабочее окно 06:00-23:00.
/// </summary>
public class BookingPeriodTests
{
    /// <summary>
    /// Период раскрывается в границы UTC той же даты - бронь не выходит за сутки.
    /// </summary>
    [Fact]
    public void Create_ValidPeriod_ExposesUtcBoundsWithinSameDay()
    {
        var period = BookingPeriod.Create(Sample.Day(), new TimeOnly(10, 0), new TimeOnly(14, 0));

        period.StartsAtUtc.Should().Be(Sample.At(10));
        period.EndsAtUtc.Should().Be(Sample.At(14));
        period.StartsAtUtc.Kind.Should().Be(DateTimeKind.Utc);
        period.EndsAtUtc.Date.Should().Be(period.StartsAtUtc.Date, "период всегда укладывается в одни сутки");
        period.Hours.Should().Be(4);
    }

    /// <summary>
    /// Границы рабочего окна доступны целиком.
    /// </summary>
    [Fact]
    public void Create_AtWindowBoundaries_Succeeds()
    {
        var period = BookingPeriod.Create(Sample.Day(), BookingPeriod.EarliestStart, BookingPeriod.LatestEnd);

        period.Hours.Should().Be(17, "с 06:00 до 23:00 ровно 17 часов");
    }

    /// <summary>
    /// Окончание должно быть позже начала.
    /// </summary>
    [Theory]
    [InlineData(14, 12)]
    [InlineData(12, 12)]
    public void Create_EndNotAfterStart_Throws(int fromHour, int toHour)
    {
        var act = () => BookingPeriod.Create(Sample.Day(), new TimeOnly(fromHour, 0), new TimeOnly(toHour, 0));

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Раньше 06:00 бронировать нельзя.
    /// </summary>
    [Fact]
    public void Create_BeforeEarliestStart_Throws()
    {
        var act = () => BookingPeriod.Create(Sample.Day(), new TimeOnly(5, 0), new TimeOnly(7, 0));

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Позже 23:00 бронировать нельзя.
    /// </summary>
    [Fact]
    public void Create_AfterLatestEnd_Throws()
    {
        var act = () => BookingPeriod.Create(Sample.Day(), new TimeOnly(22, 0), new TimeOnly(23, 30));

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Бронировать можно только по целым часам: иначе часовые слоты перестанут
    /// совпадать с уникальным индексом, который ловит пересечения.
    /// </summary>
    [Theory]
    [InlineData(10, 30, 12, 0)]
    [InlineData(10, 0, 12, 30)]
    public void Create_NotWholeHours_Throws(int fromHour, int fromMinute, int toHour, int toMinute)
    {
        var act = () => BookingPeriod.Create(
            Sample.Day(),
            new TimeOnly(fromHour, fromMinute),
            new TimeOnly(toHour, toMinute));

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Период раскладывается на часовые слоты по одному на каждый час.
    /// </summary>
    [Fact]
    public void HourlySlots_ReturnsOneSlotPerHour()
    {
        var period = BookingPeriod.Create(Sample.Day(), new TimeOnly(11, 0), new TimeOnly(14, 0));

        period.HourlySlots().Should().Equal(Sample.At(11), Sample.At(12), Sample.At(13));
    }

    /// <summary>
    /// Период - значение: два одинаковых периода равны.
    /// </summary>
    [Fact]
    public void Periods_WithSameDateAndHours_AreEqual()
    {
        var first = BookingPeriod.Create(Sample.Day(), new TimeOnly(10, 0), new TimeOnly(12, 0));
        var second = BookingPeriod.Create(Sample.Day(), new TimeOnly(10, 0), new TimeOnly(12, 0));

        second.Should().Be(first);
    }
}
