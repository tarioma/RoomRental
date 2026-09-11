using RoomRental.Application.Abstraction.Commands.Bookings;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Contracts.Requests.Bookings;
using RoomRental.Contracts.Responses;
using RoomRental.Domain.Enums;

namespace RoomRental.Mapping;

internal static class BookingMapping
{
    public static CreateBookingCommand ToCommand(this CreateBookingRequest request, Guid customerId) =>
        new(
            request.RoomId,
            customerId,
            request.Date,
            request.From,
            request.To,
            request.ServiceIds ?? []);

    public static CancelBookingCommand ToCommand(
        this CancelBookingRequest request,
        Guid bookingId,
        Guid requesterId,
        UserRole requesterRole) =>
        new(bookingId, requesterId, requesterRole, request.Reason);

    public static BookingResponse ToResponse(this BookingDto dto) =>
        new(
            dto.Id,
            dto.RoomId,
            dto.RoomName,
            dto.Date,
            dto.From,
            dto.To,
            dto.Status.ToString(),
            dto.TotalAmount,
            [.. dto.Charges.Select(ToResponse)]);

    private static BookingChargeResponse ToResponse(BookingChargeDto dto) =>
        new(
            dto.Description,
            dto.Kind.ToString(),
            dto.SegmentStart,
            dto.SegmentEnd,
            dto.Quantity,
            dto.UnitPrice,
            dto.Multiplier,
            dto.Amount);
}
