namespace RoomRental.Application.Abstraction;

/// <summary>
/// Фиксация изменений, накопленных репозиториями за один запрос.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохраняет накопленные изменения и возвращает количество затронутых записей.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
