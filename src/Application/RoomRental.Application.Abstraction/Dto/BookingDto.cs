using RoomRental.Domain.Enums;

namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Подтверждение брони с полным расчётом стоимости.
/// </summary>
/// <param name="Id">Уникальный идентификатор брони.</param>
/// <param name="RoomId">Забронированный зал.</param>
/// <param name="RoomName">Название зала.</param>
/// <param name="Date">Дата бронирования.</param>
/// <param name="From">Время начала.</param>
/// <param name="To">Время окончания.</param>
/// <param name="Status">Статус брони.</param>
/// <param name="TotalAmount">Итоговая стоимость.</param>
/// <param name="Charges">Расшифровка стоимости по позициям.</param>
public record BookingDto(
    Guid Id,
    Guid RoomId,
    string RoomName,
    DateOnly Date,
    TimeOnly From,
    TimeOnly To,
    BookingStatus Status,
    decimal TotalAmount,
    IReadOnlyList<BookingChargeDto> Charges);
