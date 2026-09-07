using RoomRental.Domain.Enums;
using RoomRental.Domain.Tests.TestData;

namespace RoomRental.Domain.Tests.Entities;

/// <summary>
/// Отмена брони: допустима только до её начала и повторяется без последствий.
/// </summary>
public class BookingCancelTests
{
    /// <summary>
    /// Будущая бронь отменяется, причина и время отмены сохраняются.
    /// </summary>
    [Fact]
    public void Cancel_FutureBooking_MarksCancelled()
    {
        var booking = Sample.Book(10, 12);

        booking.Cancel(Sample.UtcNow, "планы поменялись");

        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledAtUtc.Should().Be(Sample.UtcNow);
        booking.CancellationReason.Should().Be("планы поменялись");
    }

    /// <summary>
    /// Отмена возвращает часы зала в продажу: слоты - единственное,
    /// что мешает забронировать это время повторно.
    /// </summary>
    [Fact]
    public void Cancel_ReleasesHourlySlots()
    {
        var booking = Sample.Book(10, 12);
        booking.Slots.Should().HaveCount(2);

        booking.Cancel(Sample.UtcNow);

        booking.Slots.Should().BeEmpty("отменённая бронь не должна держать часы зала занятыми");
    }

    /// <summary>
    /// Расчёт стоимости при отмене сохраняется - он нужен для истории и отчётов.
    /// </summary>
    [Fact]
    public void Cancel_KeepsChargesAndTotal()
    {
        var booking = Sample.Book(11, 13);
        var totalBeforeCancel = booking.TotalAmount;

        booking.Cancel(Sample.UtcNow);

        booking.Charges.Should().HaveCount(2);
        booking.TotalAmount.Should().Be(totalBeforeCancel);
    }

    /// <summary>
    /// Повторная отмена ничего не меняет и не бросает исключение.
    /// </summary>
    [Fact]
    public void Cancel_AlreadyCancelled_IsIdempotent()
    {
        var booking = Sample.Book(10, 12);
        booking.Cancel(Sample.UtcNow, "первая причина");

        booking.Cancel(Sample.UtcNow.AddHours(1), "вторая причина");

        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledAtUtc.Should().Be(Sample.UtcNow, "время первой отмены не перезаписывается");
        booking.CancellationReason.Should().Be("первая причина");
    }

    /// <summary>
    /// Начавшуюся бронь отменить нельзя.
    /// </summary>
    [Fact]
    public void Cancel_AlreadyStartedBooking_Throws()
    {
        var booking = Sample.Book(10, 12);

        var act = () => booking.Cancel(booking.Period.StartsAtUtc);

        act.Should().Throw<InvalidOperationException>();
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.Slots.Should().HaveCount(2, "неудачная отмена не освобождает часы");
    }

    /// <summary>
    /// Причина отмены ограничена по длине.
    /// </summary>
    [Fact]
    public void Cancel_ReasonTooLong_Throws()
    {
        var booking = Sample.Book(10, 12);

        var act = () => booking.Cancel(Sample.UtcNow, new string('я', 101));

        act.Should().Throw<ArgumentException>();
        booking.Status.Should().Be(BookingStatus.Confirmed, "неудачная отмена не меняет состояние брони");
    }

    /// <summary>
    /// Отмена без указания причины допустима.
    /// </summary>
    [Fact]
    public void Cancel_WithoutReason_MarksCancelled()
    {
        var booking = Sample.Book(10, 12);

        booking.Cancel(Sample.UtcNow);

        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancellationReason.Should().BeNull();
    }
}
