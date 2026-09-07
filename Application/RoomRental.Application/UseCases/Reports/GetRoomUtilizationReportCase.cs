using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Queries.Reports;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Reports;

/// <summary>
/// Строит отчёт по загрузке залов за период.
/// </summary>
/// <param name="reports">Репозиторий отчётов.</param>
public class GetRoomUtilizationReportCase(
    IReportRepository reports)
    : IRequestHandler<GetRoomUtilizationReportQuery, IReadOnlyList<RoomUtilizationDto>>
{
    /// <inheritdoc />
    public Task<IReadOnlyList<RoomUtilizationDto>> Handle(GetRoomUtilizationReportQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var period = ReportPeriod.Create(request.From, request.To);

        return reports.GetRoomUtilizationAsync(period, cancellationToken);
    }
}
