namespace RoomRental.Contracts.Responses;

/// <summary>
/// Услуга, доступная при бронировании зала.
/// </summary>
/// <param name="Id">Уникальный идентификатор услуги.</param>
/// <param name="Name">Название, например «Проектор».</param>
/// <param name="Price">Стоимость за одно бронирование, в гривнах.</param>
public record ServiceResponse(Guid Id, string Name, decimal Price);
