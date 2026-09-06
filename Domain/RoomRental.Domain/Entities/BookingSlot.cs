using Ardalis.GuardClauses;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Часовой слот бронирования.
/// </summary>
public class BookingSlot
{
    private BookingSlot(Guid bookingId, Guid roomId, DateTime slotStart)
    {
        Guard.Against.Default(bookingId);
        Guard.Against.Default(roomId);
        Guard.Against.Default(slotStart);

        BookingId = bookingId;
        RoomId = roomId;
        SlotStart = slotStart;
    }
    
    /// <summary>
    /// Уникальный идентификатор бронирования.
    /// </summary>
    public Guid BookingId { get; }

    /// <summary>
    /// Бронирование.
    /// </summary>
    public Booking Booking { get; } = null!;

    /// <summary>
    /// Уникальный идентификатор бронируемого конференц-зала.
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Бронируемый конференц-зал.
    /// </summary>
    public Room Room { get; } = null!;

    /// <summary>
    /// Дата и время начала часового слота.
    /// </summary>
    public DateTime SlotStart { get; }

    /// <summary>
    /// Создаёт часовой слот брони.
    /// </summary>
    /// <param name="bookingId">Бронь.</param>
    /// <param name="roomId">Занимаемый зал.</param>
    /// <param name="slotStart">Начало часа по UTC.</param>
    public static BookingSlot Create(Guid bookingId, Guid roomId, DateTime slotStart) =>
        new(bookingId, roomId, slotStart);
}