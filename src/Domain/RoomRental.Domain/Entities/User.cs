using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using RoomRental.Domain.Enums;
using RoomRental.Domain.Extensions;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Пользователь.
/// </summary>
public class User
{
    /// <summary>
    /// Предельная длина email-адреса по RFC 5321.
    /// </summary>
    public const int MaxEmailLength = 254;

    /// <summary>
    /// Предельная длина хеша пароля. С запасом на смену алгоритма хеширования.
    /// </summary>
    public const int MaxPasswordHashLength = 256;

    private User(
        Guid id,
        string email,
        string normalizedEmail,
        string passwordHash,
        UserRole role,
        DateTime createdAtUtc)
    {
        Guard.Against.Default(id);
        Guard.Against.InvalidEmail(email);
        Guard.Against.StringTooLong(email, MaxEmailLength);
        Guard.Against.InvalidEmail(normalizedEmail);
        Guard.Against.Default(role);
        Guard.Against.EnumOutOfRange(role);
        Guard.Against.Default(createdAtUtc);

        Id = id;
        Email = email;
        NormalizedEmail = normalizedEmail;
        SetPasswordHash(passwordHash);
        Role = role;
        CreatedAtUtc = createdAtUtc;
    }

    private User()
    {
    }

    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Email-адрес.
    /// </summary>
    public string Email { get; } = null!;

    /// <summary>
    /// Нормализованный email-адрес для соблюдения уникальности.
    /// </summary>
    public string NormalizedEmail { get; } = null!;

    /// <summary>
    /// Хеш пароля.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Роль.
    /// </summary>
    public UserRole Role { get; }

    /// <summary>
    /// Дата и время создания по UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Дата и время удаления по UTC.
    /// </summary>
    public DateTime? DeletedAtUtc { get; private set; }

    /// <summary>
    /// Удалён ли пользователь.
    /// </summary>
    public bool IsDeleted => DeletedAtUtc.HasValue;

    /// <summary>
    /// Создаёт пользователя.
    /// </summary>
    /// <param name="email">Email-адрес.</param>
    /// <param name="passwordHash">Готовый хеш пароля.</param>
    /// <param name="role">Роль.</param>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public static User Create(
        string email,
        string passwordHash,
        UserRole role,
        DateTime utcNow)
    {
        return new User(
            Guid.CreateVersion7(),
            email.Trim(),
            NormalizeEmail(email),
            passwordHash,
            role,
            utcNow);
    }

    /// <summary>
    /// Приводит email к виду, в котором сравнивается уникальность.
    /// Правило одно на всё приложение: по нему строится уникальный индекс,
    /// и поиск пользователя обязан нормализовать адрес точно так же.
    /// </summary>
    public static string NormalizeEmail(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);

        return email.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Меняет хеш пароля.
    /// </summary>
    /// <param name="passwordHash">Новый хеш.</param>
    [MemberNotNull(nameof(PasswordHash))]
    public void SetPasswordHash(string passwordHash)
    {
        Guard.Against.NullOrWhiteSpace(passwordHash);
        Guard.Against.StringTooLong(passwordHash, MaxPasswordHashLength);

        PasswordHash = passwordHash.Trim();
    }

    /// <summary>
    /// Мягко удаляет пользователя. Повторный вызов ничего не меняет.
    /// </summary>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public void Delete(DateTime utcNow)
    {
        if (IsDeleted)
        {
            return;
        }

        DeletedAtUtc = utcNow;
    }
}