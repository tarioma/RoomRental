using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Хранилище услуг.
/// </summary>
public interface IServiceRepository
{
    /// <summary>
    /// Действующая услуга по идентификатору, для чтения.
    /// Изменения такого экземпляра не сохраняются.
    /// </summary>
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Действующая услуга по идентификатору, для изменения.
    /// Правки сохраняются при фиксации через <see cref="IUnitOfWork"/>.
    /// </summary>
    Task<Service?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Действующие услуги по списку идентификаторов. Ненайденные просто отсутствуют в ответе;
    /// проверку полноты выполняет <see cref="ServiceRepositoryExtensions.GetAllByIdsAsync"/>.
    /// </summary>
    Task<IReadOnlyList<Service>> GetByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Список действующих услуг постранично.
    /// </summary>
    Task<IReadOnlyList<ServiceDto>> SearchAsync(
        SearchFilters filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Помечает услугу к добавлению. Запись произойдёт при фиксации изменений.
    /// </summary>
    Task AddAsync(Service service, CancellationToken cancellationToken = default);
}
