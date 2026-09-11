using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Queries.Rooms;
using RoomRental.Contracts.Requests.Rooms;
using RoomRental.Contracts.Responses;

namespace RoomRental.Mapping;

internal static class RoomMapping
{
    public static CreateRoomCommand ToCommand(this CreateRoomRequest request) =>
        new(request.Name, request.Capacity, request.BaseHourPrice, request.ServiceIds ?? []);

    public static UpdateRoomCommand ToCommand(this UpdateRoomRequest request, Guid roomId) =>
        new(roomId, request.Name, request.Capacity, request.BaseHourPrice);

    public static SearchRoomsQuery ToQuery(this SearchRoomsRequest request) =>
        new(new SearchFilters(request.Limit, request.Offset));

    public static SearchAvailableRoomsQuery ToQuery(this SearchAvailableRoomsRequest request) =>
        new(
            request.Date,
            request.From,
            request.To,
            request.Capacity,
            new SearchFilters(request.Limit, request.Offset));

    public static RoomResponse ToResponse(this RoomDto dto) =>
        new(
            dto.Id,
            dto.Name,
            dto.Capacity,
            dto.BaseHourPrice,
            [.. dto.Services.Select(s => s.ToResponse())]);

    public static IReadOnlyList<RoomResponse> ToResponse(this IReadOnlyList<RoomDto> dtos) =>
        [.. dtos.Select(ToResponse)];
}
