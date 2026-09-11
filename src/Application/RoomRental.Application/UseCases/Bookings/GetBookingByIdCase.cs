using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Queries.Bookings;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Authorization;
using RoomRental.Application.Mapping;

namespace RoomRental.Application.UseCases.Bookings;

/// <summary>
/// Возвращает бронь с расшифровкой расчёта. Доступно владельцу и администратору.
/// </summary>
/// <param name="bookings">Репозиторий броней.</param>
public class GetBookingByIdCase(
    IBookingRepository bookings)
    : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    /// <inheritdoc />
    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var booking = await bookings.GetByIdAsync(request.BookingId, cancellationToken)
                      ?? throw new EntityNotFoundException("Бронь не найдена.");

        BookingAccess.EnsureOwnerOrAdmin(booking, request.RequesterId, request.RequesterRole);

        return BookingMapper.ToDto(booking, booking.Room);
    }
}
