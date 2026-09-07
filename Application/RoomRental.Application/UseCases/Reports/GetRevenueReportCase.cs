using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Queries.Reports;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Reports;

/// <summary>
/// Строит отчёт по выручке за период.
/// </summary>
/// <param name="reports">Репозиторий отчётов.</param>
public class GetRevenueReportCase(
    IReportRepository reports)
    : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
{
    /// <inheritdoc />
    public Task<RevenueReportDto> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var period = ReportPeriod.Create(request.From, request.To);

        return reports.GetRevenueAsync(period, cancellationToken);
    }
}
