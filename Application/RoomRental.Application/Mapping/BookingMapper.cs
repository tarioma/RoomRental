using RoomRental.Application.Abstraction.Dto;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.Mapping;

internal static class BookingMapper
{
    /// <summary>
    /// Превращает бронь в подтверждение с расшифровкой расчёта.
    /// Зал передаётся отдельно: у только что созданной брони навигация ещё не заполнена.
    /// </summary>
    public static BookingDto ToDto(Booking booking, Room room) => new(
        booking.Id,
        booking.RoomId,
        room.Name,
        booking.Period.Date,
        booking.Period.From,
        booking.Period.To,
        booking.Status,
        booking.TotalAmount,
        [.. booking.Charges.Select(c => new BookingChargeDto(
            c.Description,
            c.Kind,
            c.SegmentStart,
            c.SegmentEnd,
            c.Quantity,
            c.UnitPrice,
            c.Multiplier,
            c.Amount))]);
}
