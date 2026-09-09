namespace RoomRental.Contracts.Responses;

/// <summary>
/// Подтверждение создания сущности.
/// </summary>
/// <param name="Id">Уникальный идентификатор созданной записи.</param>
public record CreatedResponse(Guid Id);
