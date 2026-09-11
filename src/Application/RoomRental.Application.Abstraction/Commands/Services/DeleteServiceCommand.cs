using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Services;

/// <summary>
/// Мягкое удаление услуги: запись остаётся, но заказать услугу больше нельзя.
/// </summary>
/// <param name="ServiceId">Удаляемая услуга.</param>
public record DeleteServiceCommand(Guid ServiceId) : IRequest;
