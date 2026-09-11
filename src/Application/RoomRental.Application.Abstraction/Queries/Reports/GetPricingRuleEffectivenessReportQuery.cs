using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Reports;

/// <summary>
/// Эффективность правил ценообразования: что скидки и наценки дали за период.
/// </summary>
/// <param name="From">Начало периода, включительно.</param>
/// <param name="To">Окончание периода, включительно.</param>
public record GetPricingRuleEffectivenessReportQuery(
    DateOnly From,
    DateOnly To)
    : IRequest<IReadOnlyList<PricingRuleEffectivenessDto>>;
