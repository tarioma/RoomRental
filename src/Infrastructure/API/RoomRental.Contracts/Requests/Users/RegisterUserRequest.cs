using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Users;

/// <summary>
/// Данные для регистрации клиента.
/// </summary>
/// <param name="Email">Email-адрес. Уникален без учёта регистра и пробелов по краям.</param>
/// <param name="Password">Пароль, не короче 8 символов. Хранится только в виде хеша.</param>
public record RegisterUserRequest(
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MinLength(8), MaxLength(128)] string Password);
