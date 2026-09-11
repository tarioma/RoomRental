using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Services;

/// <summary>
/// Новые данные услуги. Передаются полностью, а не частично.
/// </summary>
/// <param name="Name">Название услуги. До 100 символов.</param>
/// <param name="Price">Стоимость услуги за одно бронирование, в гривнах.</param>
public record UpdateServiceRequest(
    [Required, MaxLength(100)] string Name,
    [Range(0, 1_000_000)] decimal Price);
