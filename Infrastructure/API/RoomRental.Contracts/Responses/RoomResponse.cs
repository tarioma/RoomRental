namespace RoomRental.Contracts.Responses;

/// <summary>
/// Конференц-зал вместе с доступными в нём услугами.
/// </summary>
/// <param name="Id">Уникальный идентификатор зала.</param>
/// <param name="Name">Название, например «Зал А».</param>
/// <param name="Capacity">Вместимость в людях.</param>
/// <param name="BaseHourPrice">
/// Базовая стоимость аренды за час. Итоговая цена брони считается от неё
/// с учётом множителей правил ценообразования.
/// </param>
/// <param name="Services">Услуги, которые можно заказать при бронировании этого зала.</param>
public record RoomResponse(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourPrice,
    IReadOnlyList<ServiceResponse> Services);
