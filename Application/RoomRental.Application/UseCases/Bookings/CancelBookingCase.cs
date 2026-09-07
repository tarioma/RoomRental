using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Bookings;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Authorization;

namespace RoomRental.Application.UseCases.Bookings;

/// <summary>
/// Отменяет бронь. Правила отмены - в самой сущности: повторная отмена проходит без последствий,
/// а начавшуюся бронь отменить нельзя.
/// </summary>
public class CancelBookingCase(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CancelBookingCommand>
{
    /// <inheritdoc />
    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var booking = await bookings.GetForUpdateAsync(request.BookingId, cancellationToken)
                      ?? throw new EntityNotFoundException("Бронь не найдена.");

        BookingAccess.EnsureOwnerOrAdmin(booking, request.RequesterId, request.RequesterRole);

        booking.Cancel(timeProvider.GetUtcNow().UtcDateTime, request.Reason);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
