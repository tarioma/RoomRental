using MediatR;
using RoomRental.Domain.Enums;

namespace RoomRental.Application.Abstraction.Commands.Bookings;

/// <summary>
/// Отмена брони. Допустима владельцу брони и администратору,
/// и только пока бронь не началась.
/// </summary>
/// <param name="BookingId">Отменяемая бронь.</param>
/// <param name="RequesterId">Пользователь, выполняющий отмену.</param>
/// <param name="RequesterRole">Его роль.</param>
/// <param name="Reason">Причина отмены, необязательна.</param>
public record CancelBookingCommand(
    Guid BookingId,
    Guid RequesterId,
    UserRole RequesterRole,
    string? Reason = null)
    : IRequest;
