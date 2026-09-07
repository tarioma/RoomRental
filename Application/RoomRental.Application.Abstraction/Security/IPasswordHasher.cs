namespace RoomRental.Application.Abstraction.Security;

/// <summary>
/// Хеширование паролей. Алгоритм скрыт за интерфейсом,
/// чтобы его смена не задевала слой приложения.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Возвращает хеш пароля вместе со случайной солью.
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Проверяет пароль против ранее посчитанного хеша.
    /// </summary>
    bool Verify(string password, string hash);
}
