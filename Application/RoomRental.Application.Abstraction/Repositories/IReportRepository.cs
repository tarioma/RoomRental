using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Выборки для отчётов.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Выручка за период с разрезами по залам и услугам.
    /// </summary>
    Task<RevenueReportDto> GetRevenueAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Загрузка залов за период. Считается по броням, а не по слотам:
    /// слоты освобождаются при отмене и историю не хранят.
    /// </summary>
    Task<IReadOnlyList<RoomUtilizationDto>> GetRoomUtilizationAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Что каждое правило ценообразования принесло или стоило за период.
    /// </summary>
    Task<IReadOnlyList<PricingRuleEffectivenessDto>> GetPricingRuleEffectivenessAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default);
}
