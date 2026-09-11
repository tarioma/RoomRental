namespace RoomRental.Infrastructure.Seeding;

/// <summary>
/// Параметры наполнения базы начальными данными.
/// </summary>
public class SeedOptions
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    public const string SectionName = "Seed";

    /// <summary>
    /// Минимальная длина пароля администратора.
    /// </summary>
    public const int MinAdminPasswordLength = 8;

    /// <summary>
    /// Наполнять ли базу при старте. Операция идемпотентна, но в проде её обычно выключают.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Email администратора, создаваемого при наполнении базы.
    /// </summary>
    public string AdminEmail { get; init; } = string.Empty;

    /// <summary>
    /// Его пароль. Хранится вне репозитория.
    /// </summary>
    public string AdminPassword { get; init; } = string.Empty;
}
