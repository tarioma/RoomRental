using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Reports;

/// <summary>
/// Загрузка залов за период: сколько часов из доступных выкуплено.
/// </summary>
/// <param name="From">Начало периода, включительно.</param>
/// <param name="To">Окончание периода, включительно.</param>
public record GetRoomUtilizationReportQuery(
    DateOnly From,
    DateOnly To)
    : IRequest<IReadOnlyList<RoomUtilizationDto>>;
