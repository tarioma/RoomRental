namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Токен доступа и момент его истечения.
/// </summary>
public record AccessTokenDto(string Token, DateTime ExpiresAtUtc);
