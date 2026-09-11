using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Bookings;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Mapping;
using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Application.UseCases.Bookings;

/// <summary>
/// Бронирует зал: проверяет занятость, считает стоимость по действующим правилам
/// и возвращает подтверждение с расшифровкой расчёта.
/// </summary>
public class CreateBookingCase(
    IRoomRepository rooms,
    IServiceRepository services,
    IBookingRepository bookings,
    IPricingRuleRepository pricingRules,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreateBookingCommand, BookingDto>
{
    /// <inheritdoc />
    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.Null(request.ServiceIds);

        var period = BookingPeriod.Create(request.Date, request.From, request.To);

        var room = await rooms.GetByIdAsync(request.RoomId, cancellationToken)
                   ?? throw new EntityNotFoundException("Зал не найден.");

        var requestedServices = await services.GetAllByIdsAsync(request.ServiceIds, cancellationToken);

        // Ранняя проверка
        if (await bookings.HasOverlappingSlotsAsync(room.Id, period, cancellationToken))
        {
            throw new SlotAlreadyBookedException("Зал уже забронирован на выбранное время.");
        }

        var rules = await pricingRules.GetActiveAsync(cancellationToken);

        var booking = Booking.Create(
            room,
            period,
            requestedServices,
            rules,
            request.CustomerId,
            timeProvider.GetUtcNow().UtcDateTime);

        await bookings.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BookingMapper.ToDto(booking, room);
    }
}
