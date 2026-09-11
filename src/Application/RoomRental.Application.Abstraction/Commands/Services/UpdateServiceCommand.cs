using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Services;

/// <summary>
/// Обновление данных услуги. На стоимость уже созданных броней не влияет:
/// цена в них зафиксирована на момент бронирования.
/// </summary>
/// <param name="ServiceId">Изменяемая услуга.</param>
/// <param name="Name">Новое название.</param>
/// <param name="Price">Новая стоимость.</param>
public record UpdateServiceCommand(Guid ServiceId, string Name, decimal Price) : IRequest;
