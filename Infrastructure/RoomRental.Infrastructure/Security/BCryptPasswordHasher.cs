using RoomRental.Application.Abstraction.Security;

namespace RoomRental.Infrastructure.Security;

/// <summary>
/// Хеширование паролей алгоритмом BCrypt.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    /// <inheritdoc />
    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
