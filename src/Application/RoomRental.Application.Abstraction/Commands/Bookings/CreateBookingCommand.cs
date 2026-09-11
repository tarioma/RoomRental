using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Commands.Bookings;

/// <summary>
/// Бронирование зала на указанный период с набором услуг.
/// </summary>
/// <param name="RoomId">Бронируемый зал.</param>
/// <param name="CustomerId">Клиент, оформляющий бронь.</param>
/// <param name="Date">Дата бронирования.</param>
/// <param name="From">Время начала.</param>
/// <param name="To">Время окончания.</param>
/// <param name="ServiceIds">Заказанные услуги, каждая должна предоставляться залом.</param>
public record CreateBookingCommand(
    Guid RoomId,
    Guid CustomerId,
    DateOnly Date,
    TimeOnly From,
    TimeOnly To,
    IReadOnlyList<Guid> ServiceIds)
    : IRequest<BookingDto>;
