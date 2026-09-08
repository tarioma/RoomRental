using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Хранилище пользователей.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class UserRepository(DatabaseContext context) : IUserRepository
{
    /// <inheritdoc />
    public Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                u => u.NormalizedEmail == normalizedEmail && u.DeletedAtUtc == null,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);

        return Task.CompletedTask;
    }
}
