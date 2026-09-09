using RoomRental.Application.Abstraction.Commands.Services;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Queries.Services;
using RoomRental.Contracts.Requests.Services;
using RoomRental.Contracts.Responses;

namespace RoomRental.Mapping;

internal static class ServiceMapping
{
    public static CreateServiceCommand ToCommand(this CreateServiceRequest request) =>
        new(request.Name, request.Price);

    public static UpdateServiceCommand ToCommand(this UpdateServiceRequest request, Guid serviceId) =>
        new(serviceId, request.Name, request.Price);

    public static SearchServicesQuery ToQuery(this SearchServicesRequest request) =>
        new(new SearchFilters(request.Limit, request.Offset));

    public static ServiceResponse ToResponse(this ServiceDto dto) =>
        new(dto.Id, dto.Name, dto.Price);

    public static IReadOnlyList<ServiceResponse> ToResponse(this IReadOnlyList<ServiceDto> dtos) =>
        [.. dtos.Select(ToResponse)];
}
