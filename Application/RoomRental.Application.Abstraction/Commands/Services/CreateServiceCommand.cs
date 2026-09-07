using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Services;

/// <summary>
/// Создание услуги.
/// </summary>
/// <param name="Name">Название услуги.</param>
/// <param name="Price">Стоимость за одно бронирование.</param>
public record CreateServiceCommand(string Name, decimal Price) : IRequest<Guid>;
