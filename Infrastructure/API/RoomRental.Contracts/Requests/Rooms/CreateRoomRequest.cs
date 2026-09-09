using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Rooms;

/// <summary>
/// Данные для создания конференц-зала.
/// </summary>
/// <param name="Name">Название зала, например «Зал А». До 100 символов.</param>
/// <param name="Capacity">Вместимость в людях, от 1 до 100.</param>
/// <param name="BaseHourPrice">
/// Базовая стоимость аренды за час в гривнах. К ней применяются
/// множители правил ценообразования в зависимости от времени суток.
/// </param>
/// <param name="ServiceIds">
/// Услуги, доступные в этом зале. Заказать при бронировании можно только их.
/// Можно не передавать - тогда зал создаётся без услуг.
/// </param>
public record CreateRoomRequest(
    [Required, MaxLength(100)] string Name,
    [Range(1, 100)] int Capacity,
    [Range(0.01, 1_000_000)] decimal BaseHourPrice,
    IReadOnlyList<Guid>? ServiceIds);
