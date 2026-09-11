using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Хранилище правил ценообразования.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class PricingRuleRepository(DatabaseContext context) : IPricingRuleRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<PricingRule>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.PricingRules
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }
}
