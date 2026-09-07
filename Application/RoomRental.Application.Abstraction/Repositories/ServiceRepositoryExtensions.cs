using Ardalis.GuardClauses;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Дополнения к хранилищу услуг.
/// </summary>
public static class ServiceRepositoryExtensions
{
    /// <summary>
    /// Загружает услуги по идентификаторам и убеждается, что найдены все до единой.
    /// Повторяющиеся идентификаторы схлопываются, поэтому дубли в запросе не считаются ошибкой.
    /// </summary>
    /// <exception cref="EntityNotFoundException">Хотя бы одна услуга не найдена.</exception>
    public static async Task<IReadOnlyList<Service>> GetAllByIdsAsync(
        this IServiceRepository services,
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(ids);

        if (ids.Count == 0)
        {
            return [];
        }

        var requested = ids.Distinct().ToList();
        var found = await services.GetByIdsAsync(requested, cancellationToken);

        if (found.Count == requested.Count)
        {
            return found;
        }

        var missing = requested.Except(found.Select(s => s.Id));

        throw new EntityNotFoundException($"Услуги не найдены: {string.Join(", ", missing)}.");
    }
}
