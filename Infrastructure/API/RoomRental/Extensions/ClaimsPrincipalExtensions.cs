using System.Security.Claims;
using RoomRental.Domain.Enums;

namespace RoomRental.Extensions;

internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Идентификатор текущего пользователя из токена.
    /// Вызывается только под <c>[Authorize]</c>, поэтому отсутствие клейма - ошибка конфигурации.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("В токене нет идентификатора пользователя.");
    }

    /// <summary>
    /// Роль текущего пользователя из токена.
    /// Нераспознанное значение трактуется как <see cref="UserRole.Undefined"/>,
    /// то есть без привилегий.
    /// </summary>
    public static UserRole GetUserRole(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.Role);

        return Enum.TryParse<UserRole>(value, out var role) ? role : UserRole.Undefined;
    }
}
