using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Domain.Enums;

namespace RoomRental.Application.Abstraction.Queries.Bookings;

/// <summary>
/// Бронь с полной расшифровкой расчёта стоимости.
/// Доступна владельцу брони и администратору.
/// </summary>
/// <param name="BookingId">Запрашиваемая бронь.</param>
/// <param name="RequesterId">Пользователь, выполняющий запрос.</param>
/// <param name="RequesterRole">Его роль.</param>
public record GetBookingByIdQuery(
    Guid BookingId,
    Guid RequesterId,
    UserRole RequesterRole)
    : IRequest<BookingDto>;
