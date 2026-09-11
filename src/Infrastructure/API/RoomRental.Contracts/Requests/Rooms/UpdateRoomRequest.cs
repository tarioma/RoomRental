using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Rooms;

/// <summary>
/// Новые данные зала. Передаются полностью, а не частично.
/// Состав услуг меняется отдельными методами.
/// </summary>
/// <param name="Name">Название зала. До 100 символов.</param>
/// <param name="Capacity">Вместимость в людях, от 1 до 100.</param>
/// <param name="BaseHourPrice">Базовая стоимость аренды за час в гривнах.</param>
public record UpdateRoomRequest(
    [Required, MaxLength(100)] string Name,
    [Range(1, 100)] int Capacity,
    [Range(0.01, 1_000_000)] decimal BaseHourPrice);
