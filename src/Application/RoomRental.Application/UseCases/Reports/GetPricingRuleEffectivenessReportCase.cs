using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Queries.Reports;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Reports;

/// <summary>
/// Строит отчёт по эффективности правил ценообразования.
/// </summary>
/// <param name="reports">Репозиторий отчётов.</param>
public class GetPricingRuleEffectivenessReportCase(
    IReportRepository reports)
    : IRequestHandler<GetPricingRuleEffectivenessReportQuery, IReadOnlyList<PricingRuleEffectivenessDto>>
{
    /// <inheritdoc />
    public Task<IReadOnlyList<PricingRuleEffectivenessDto>> Handle(GetPricingRuleEffectivenessReportQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var period = ReportPeriod.Create(request.From, request.To);

        return reports.GetPricingRuleEffectivenessAsync(period, cancellationToken);
    }
}
