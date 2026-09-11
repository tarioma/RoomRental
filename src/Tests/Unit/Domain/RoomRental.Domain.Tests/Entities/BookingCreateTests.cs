using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;
using RoomRental.Domain.Enums;
using RoomRental.Domain.Tests.TestData;

namespace RoomRental.Domain.Tests.Entities;

/// <summary>
/// Создание брони: итоговая стоимость, часовые слоты, услуги и правила периода.
/// </summary>
public class BookingCreateTests
{
    /// <summary>
    /// Итог брони равен сумме всех позиций счёта - отдельного источника правды нет.
    /// </summary>
    [Fact]
    public void Create_TotalAmount_EqualsSumOfCharges()
    {
        var booking = Sample.Book(11, 13);

        booking.TotalAmount.Should().Be(booking.Charges.Sum(c => c.Amount));
        booking.TotalAmount.Should().Be(4300m);
    }

    /// <summary>
    /// Пример из ТЗ: 12:00-14:00 в зале А с проектором - 2 часа пиковых плюс услуга.
    /// </summary>
    [Fact]
    public void Create_WithService_AddsServiceChargeToTotal()
    {
        var projector = Sample.Projector();

        var booking = Sample.Book(12, 14, room: null, projector);

        booking.TotalAmount.Should().Be(5100m, "2 часа по 2300 за пиковые плюс 500 за проектор");

        var serviceCharge = booking.Charges.Should().ContainSingle(c => c.Kind == ChargeKind.Service).Which;
        serviceCharge.ServiceId.Should().Be(projector.Id);
        serviceCharge.Amount.Should().Be(500m);
        serviceCharge.Quantity.Should().Be(1m);
    }

    /// <summary>
    /// Цена услуги копируется в позицию счёта, поэтому её последующее изменение
    /// не переписывает стоимость уже созданной брони.
    /// </summary>
    [Fact]
    public void Create_ServiceCharge_SnapshotsPriceAtBookingTime()
    {
        var projector = Sample.Projector();
        var booking = Sample.Book(10, 11, room: null, projector);

        projector.SetPrice(9999m);

        booking.Charges.Should().ContainSingle(c => c.Kind == ChargeKind.Service)
            .Which.UnitPrice.Should().Be(500m, "цена зафиксирована на момент бронирования");
        booking.TotalAmount.Should().Be(2500m);
    }

    /// <summary>
    /// Одна и та же услуга, переданная дважды, тарифицируется один раз.
    /// </summary>
    [Fact]
    public void Create_DuplicateServices_AreChargedOnce()
    {
        var projector = Sample.Projector();

        var booking = Sample.Book(10, 11, room: null, projector, projector);

        booking.Charges.Should().ContainSingle(c => c.Kind == ChargeKind.Service);
        booking.TotalAmount.Should().Be(2500m);
    }

    /// <summary>
    /// На каждый час брони создаётся ровно один слот того же зала -
    /// на них держится защита от двойного бронирования.
    /// </summary>
    [Fact]
    public void Create_ProducesOneSlotPerHour()
    {
        var room = Sample.RoomA();

        var booking = Sample.Book(11, 14, room);

        booking.Slots.Select(s => s.SlotStart)
            .Should().Equal(Sample.At(11), Sample.At(12), Sample.At(13));
        booking.Slots.Should().OnlyContain(s => s.RoomId == room.Id);
        booking.Slots.Should().OnlyContain(s => s.BookingId == booking.Id);
    }

    /// <summary>
    /// Период сохраняется в брони целиком и остаётся в пределах одних суток.
    /// </summary>
    [Fact]
    public void Create_StoresBookingPeriod()
    {
        var booking = Sample.Book(11, 14);

        booking.Period.Should().Be(Sample.Period(11, 14));
        booking.Period.Date.Should().Be(Sample.Day());
        booking.Period.Hours.Should().Be(3);
        booking.Period.StartsAtUtc.Should().Be(Sample.At(11));
        booking.Period.EndsAtUtc.Should().Be(Sample.At(14));
    }

    /// <summary>
    /// Новая бронь всегда подтверждена - статус не приходит снаружи.
    /// </summary>
    [Fact]
    public void Create_NewBooking_IsConfirmed()
    {
        var booking = Sample.Book(10, 12);

        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.CancelledAtUtc.Should().BeNull();
        booking.CreatedAtUtc.Should().Be(Sample.UtcNow);
    }

    /// <summary>
    /// Заказать можно только услугу, которую предоставляет зал.
    /// </summary>
    [Fact]
    public void Create_ServiceNotOfferedByRoom_Throws()
    {
        var roomWithProjectorOnly = Sample.RoomA(Sample.Projector());

        var act = () => Sample.Book(10, 11, roomWithProjectorOnly, Sample.Wifi());

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Удалённую услугу нельзя добавить в бронь.
    /// </summary>
    [Fact]
    public void Create_DeletedService_Throws()
    {
        var projector = Sample.Projector();
        var room = Sample.RoomA(projector);
        projector.Delete(Sample.UtcNow);

        var act = () => Sample.Book(10, 11, room, projector);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Удалённый зал забронировать нельзя.
    /// </summary>
    [Fact]
    public void Create_DeletedRoom_Throws()
    {
        var room = Sample.RoomA();
        room.Delete(Sample.UtcNow);

        var act = () => Sample.Book(10, 11, room);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Границы окна бронирования 06:00-23:00 включительно доступны.
    /// </summary>
    [Theory]
    [InlineData(6, 7)]
    [InlineData(22, 23)]
    [InlineData(6, 23)]
    public void Create_WithinBookingWindow_Succeeds(int fromHour, int toHour)
    {
        var booking = Sample.Book(fromHour, toHour);

        booking.Slots.Should().HaveCount(toHour - fromHour);
    }

    /// <summary>
    /// Бронь на прошедшее время невозможна.
    /// </summary>
    [Fact]
    public void Create_InThePast_Throws()
    {
        var act = () => Booking.Create(
            Sample.RoomA(),
            Sample.Period(10, 12, dayOffset: -1),
            [],
            Sample.PricingRules(),
            Sample.CustomerId,
            Sample.UtcNow);

        act.Should().Throw<ArgumentException>();
    }
}
