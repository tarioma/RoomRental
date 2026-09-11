using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Хранилище услуг.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class ServiceRepository(DatabaseContext context) : IServiceRepository
{
    /// <inheritdoc />
    public Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Services
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id && s.DeletedAtUtc == null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Service?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Services
            .SingleOrDefaultAsync(s => s.Id == id && s.DeletedAtUtc == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Service>> GetByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        // Отслеживание намеренное: найденные услуги попадают в состав нового зала,
        // и без него Entity Framework счёл бы их новыми записями и вставил заново.
        return await context.Services
            .Where(s => ids.Contains(s.Id) && s.DeletedAtUtc == null)
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceDto>> SearchAsync(
        SearchFilters filters,
        CancellationToken cancellationToken = default)
    {
        return await context.Services
            .AsNoTracking()
            .Where(s => s.DeletedAtUtc == null)
            .OrderBy(s => s.Name)
            .ThenBy(s => s.Id)
            .Skip(filters.Offset)
            .Take(filters.Limit)
            .Select(s => new ServiceDto(s.Id, s.Name, s.Price))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Service service, CancellationToken cancellationToken = default)
    {
        context.Services.Add(service);

        return Task.CompletedTask;
    }

}
