using RoomRental.Application.Abstraction.Dto;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.Abstraction.Security;

/// <summary>
/// Выпуск токенов доступа. Формат токена скрыт за интерфейсом.
/// </summary>
public interface IAccessTokenGenerator
{
    /// <summary>
    /// Выпускает токен доступа для пользователя.
    /// </summary>
    /// <param name="user">Пользователь, для которого выпускается токен.</param>
    AccessTokenDto Generate(User user);
}
