using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Bookings;

/// <summary>
/// Данные для бронирования зала. Клиент определяется по токену,
/// передавать его в теле запроса не нужно.
/// </summary>
/// <param name="RoomId">Бронируемый зал.</param>
/// <param name="Date">Дата бронирования. Бронь не может переходить на следующие сутки.</param>
/// <param name="From">Время начала, целый час не раньше 06:00.</param>
/// <param name="To">Время окончания, целый час не позже 23:00.</param>
/// <param name="ServiceIds">
/// Заказанные услуги. Каждая должна предоставляться этим залом.
/// Можно не передавать - тогда бронируется только зал.
/// </param>
public record CreateBookingRequest(
    [Required] Guid RoomId,
    [Required] DateOnly Date,
    [Required] TimeOnly From,
    [Required] TimeOnly To,
    IReadOnlyList<Guid>? ServiceIds);
