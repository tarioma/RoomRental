using RoomRental.Domain.Entities;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Хранилище правил ценообразования.
/// </summary>
public interface IPricingRuleRepository
{
    /// <summary>
    /// Действующие правила ценообразования, применяемые при расчёте стоимости аренды.
    /// </summary>
    Task<IReadOnlyList<PricingRule>> GetActiveAsync(CancellationToken cancellationToken = default);
}
