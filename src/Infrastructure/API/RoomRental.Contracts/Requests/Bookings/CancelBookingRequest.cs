using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Bookings;

/// <summary>
/// Данные для отмены брони.
/// </summary>
/// <param name="Reason">Причина отмены, до 100 символов. Можно не указывать.</param>
public record CancelBookingRequest([MaxLength(100)] string? Reason);
