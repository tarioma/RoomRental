namespace RoomRental.Infrastructure.Security;

/// <summary>
/// Параметры выпуска и проверки токенов доступа.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Минимальная длина ключа для HMAC-SHA256.
    /// </summary>
    public const int MinKeyLength = 32;

    /// <summary>
    /// Издатель токена. Проверяется при валидации.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Получатель токена. Проверяется при валидации.
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Секретный ключ подписи. Хранится вне репозитория.
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Срок жизни токена в минутах.
    /// </summary>
    public int LifetimeMinutes { get; init; } = 60;
}
