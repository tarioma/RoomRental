using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Users;

/// <summary>
/// Данные для входа.
/// </summary>
/// <param name="Email">Email-адрес, указанный при регистрации. Регистр не важен.</param>
/// <param name="Password">Пароль.</param>
public record LoginRequest(
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MaxLength(128)] string Password);
