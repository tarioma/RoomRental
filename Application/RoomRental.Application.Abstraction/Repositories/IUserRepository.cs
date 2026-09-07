using RoomRental.Domain.Entities;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Хранилище пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Занят ли email. Принимает уже нормализованное значение.
    /// </summary>
    Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Действующий пользователь по нормализованному email либо <c>null</c>.
    /// </summary>
    Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Помечает пользователя к добавлению.
    /// </summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
