using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Domain.Services;
using RoomRental.Domain.ValueObjects;
using RoomRental.Domain.Tests.TestData;

namespace RoomRental.Domain.Tests.Services;

/// <summary>
/// Расчёт стоимости аренды зала - ядро ТЗ.
/// Проверяются разбиение периода на сегменты, выбор правила и суммы из ТЗ.
/// </summary>
public class RoomTimePricingTests
{
    private static readonly Guid BookingId = Guid.CreateVersion7();

    private static IReadOnlyList<BookingCharge> Split(int fromHour, int toHour, IReadOnlyList<PricingRule>? rules = null) =>
        RoomTimePricing.Split(
            BookingId,
            Sample.Period(fromHour, toHour),
            Sample.BaseHourPrice,
            rules ?? Sample.PricingRules());

    /// <summary>
    /// 11:00-13:00 - час стандартных часов и час пиковых: 2000 + 2300.
    /// </summary>
    [Fact]
    public void Split_StandardThenPeak_SplitsIntoTwoSegments()
    {
        var charges = Split(11, 13);

        charges.Should().HaveCount(2);

        charges[0].Description.Should().Be("Стандартные часы");
        charges[0].Multiplier.Should().Be(1.00m);
        charges[0].Amount.Should().Be(2000m);

        charges[1].Description.Should().Be("Пиковые часы");
        charges[1].Multiplier.Should().Be(1.15m);
        charges[1].Amount.Should().Be(2300m);

        charges.Sum(c => c.Amount).Should().Be(4300m, "час стандартных 2000 плюс час пиковых с наценкой 15%");
    }

    /// <summary>
    /// 17:00-19:00 - час стандартных часов и час вечерних со скидкой 20%: 2000 + 1600.
    /// </summary>
    [Fact]
    public void Split_StandardThenEvening_SplitsIntoTwoSegments()
    {
        var charges = Split(17, 19);

        charges.Should().HaveCount(2);
        charges[0].Amount.Should().Be(2000m);
        charges[1].Multiplier.Should().Be(0.80m);
        charges[1].Amount.Should().Be(1600m);

        charges.Sum(c => c.Amount).Should().Be(3600m, "вечерние часы идут со скидкой 20%");
    }

    /// <summary>
    /// 08:00-10:00 - час утренних часов со скидкой 10% и час стандартных: 1800 + 2000.
    /// </summary>
    [Fact]
    public void Split_MorningThenStandard_SplitsIntoTwoSegments()
    {
        var charges = Split(8, 10);

        charges.Should().HaveCount(2);
        charges[0].Multiplier.Should().Be(0.90m);
        charges[0].Amount.Should().Be(1800m);
        charges[1].Amount.Should().Be(2000m);

        charges.Sum(c => c.Amount).Should().Be(3800m, "утренние часы идут со скидкой 10%");
    }

    /// <summary>
    /// Соседние часы с одним правилом склеиваются в один сегмент,
    /// а не превращаются в отдельную позицию счёта на каждый час.
    /// </summary>
    [Fact]
    public void Split_AdjacentHoursWithSameRule_MergeIntoSingleSegment()
    {
        var charge = Split(9, 12).Should().ContainSingle("три часа подряд подпадают под одно правило").Which;

        charge.Description.Should().Be("Стандартные часы");
        charge.Quantity.Should().Be(3m);
        charge.SegmentStart.Should().Be(Sample.At(9));
        charge.SegmentEnd.Should().Be(Sample.At(12));
        charge.Amount.Should().Be(6000m);
    }

    /// <summary>
    /// Пиковые часы лежат внутри стандартных - побеждает правило с большим приоритетом.
    /// </summary>
    [Fact]
    public void Split_OverlappingRules_HigherPriorityWins()
    {
        var charge = Split(12, 14).Should().ContainSingle().Which;

        charge.Description.Should().Be("Пиковые часы", "приоритет пиковых часов выше, чем у стандартных");
        charge.Quantity.Should().Be(2m);
        charge.Amount.Should().Be(4600m);
    }

    /// <summary>
    /// Границы правил полуоткрытые: 18:00 относится к вечерним часам, а не к стандартным.
    /// </summary>
    [Fact]
    public void Split_AtRuleBoundary_UsesRuleThatStartsAtThatHour()
    {
        var charge = Split(18, 19).Should().ContainSingle().Which;

        charge.Description.Should().Be("Вечерние часы", "граница правила включает начало и не включает конец");
        charge.Amount.Should().Be(1600m);
    }

    /// <summary>
    /// Часы, не покрытые ни одним правилом, тарифицируются по базовой ставке без множителя.
    /// </summary>
    [Fact]
    public void Split_HoursWithoutRule_FallBackToBaseRate()
    {
        var onlyPeak = Sample.PricingRules().Where(r => r.Name == "Пиковые часы").ToList();

        var charge = Split(9, 12, onlyPeak).Should().ContainSingle().Which;

        charge.PricingRuleId.Should().BeNull("на эти часы не заведено ни одного правила");
        charge.Description.Should().Be(
            BookingCharge.BaseRateDescription,
            "часы без правила не должны выдавать себя за часы настоящего правила");
        charge.Multiplier.Should().Be(1m);
        charge.Quantity.Should().Be(3m);
        charge.Amount.Should().Be(6000m);
    }

    /// <summary>
    /// Выключенное правило в расчёте не участвует - период уходит следующему по приоритету.
    /// </summary>
    [Fact]
    public void Split_InactiveRule_IsIgnored()
    {
        var rules = Sample.PricingRules();
        rules.Single(r => r.Name == "Пиковые часы").IsActive = false;

        var charge = Split(12, 14, rules).Should().ContainSingle().Which;

        charge.Description.Should().Be("Стандартные часы");
        charge.Amount.Should().Be(4000m, "без пиковой наценки остаётся базовая ставка");
    }

    /// <summary>
    /// Каждая позиция счёта привязана к своей броне и относится к аренде зала.
    /// </summary>
    [Fact]
    public void Split_AllCharges_BelongToBookingAndAreRoomTime()
    {
        var charges = Split(8, 19);

        charges.Should().OnlyContain(c => c.BookingId == BookingId);
        charges.Should().OnlyContain(c => c.Kind == ChargeKind.RoomTime);
        charges.Should().OnlyContain(c => c.UnitPrice == Sample.BaseHourPrice);
    }

    /// <summary>
    /// Сегменты идут подряд и без разрывов покрывают весь период брони.
    /// </summary>
    [Fact]
    public void Split_Segments_CoverWholePeriodWithoutGaps()
    {
        var charges = Split(6, 23);

        charges[0].SegmentStart.Should().Be(Sample.At(6));
        charges[^1].SegmentEnd.Should().Be(Sample.At(23));

        charges.Zip(charges.Skip(1))
            .Should().OnlyContain(pair => pair.First.SegmentEnd == pair.Second.SegmentStart,
                "сегменты должны стыковаться без разрывов и наложений");

        charges.Sum(c => c.Quantity).Should().Be(17m, "с 06:00 до 23:00 ровно 17 часов");
    }
}
