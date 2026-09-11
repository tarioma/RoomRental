namespace RoomRental.Domain.Enums;

/// <summary>
/// Состояние брони.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Значение не задано. Признак незаполненных данных.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Бронь подтверждена и занимает часы зала.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Бронь отменена, занятые часы освобождены.
    /// </summary>
    Cancelled = 2
}