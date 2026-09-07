using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Services;

/// <summary>
/// Услуга по идентификатору. Возвращает <c>null</c>, если услуга не найдена или удалена.
/// </summary>
/// <param name="ServiceId">Искомая услуга.</param>
public record GetServiceByIdQuery(Guid ServiceId) : IRequest<ServiceDto?>;
