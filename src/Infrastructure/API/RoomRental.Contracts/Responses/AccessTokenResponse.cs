namespace RoomRental.Contracts.Responses;

/// <summary>
/// Токен доступа. Передаётся в заголовке Authorization: Bearer {token}.
/// </summary>
/// <param name="Token">Сам токен.</param>
/// <param name="ExpiresAtUtc">Момент истечения по UTC. После него нужен повторный вход.</param>
public record AccessTokenResponse(string Token, DateTime ExpiresAtUtc);
