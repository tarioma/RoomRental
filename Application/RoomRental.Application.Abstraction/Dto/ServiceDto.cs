namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Услуга, доступная при бронировании зала.
/// </summary>
/// <param name="Id">Уникальный идентификатор услуги.</param>
/// <param name="Name">Название.</param>
/// <param name="Price">Стоимость за одно бронирование.</param>
public record ServiceDto(Guid Id, string Name, decimal Price);
