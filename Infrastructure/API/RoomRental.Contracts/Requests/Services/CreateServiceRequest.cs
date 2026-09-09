using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Services;

/// <summary>
/// Данные для создания услуги.
/// </summary>
/// <param name="Name">Название услуги. До 100 символов.</param>
/// <example>Проектор</example>
/// <param name="Price">Стоимость услуги за одно бронирование, в гривнах. Ноль допустим.</param>
public record CreateServiceRequest(
    [Required, MaxLength(100)] string Name,
    [Range(0, 1_000_000)] decimal Price);
