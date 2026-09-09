using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Rooms;

/// <summary>
/// Параметры подбора залов, свободных на указанное время.
/// Период подчиняется тем же правилам, что и бронирование:
/// одни сутки, целые часы, с 06:00 до 23:00.
/// </summary>
public record SearchAvailableRoomsRequest : PagedRequest
{
    /// <summary>
    /// Дата бронирования.
    /// </summary>
    /// <example>2026-09-15</example>
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Время начала, целый час не раньше 06:00.
    /// </summary>
    /// <example>10:00</example>
    [Required]
    public TimeOnly From { get; init; }

    /// <summary>
    /// Время окончания, целый час не позже 23:00.
    /// </summary>
    /// <example>14:00</example>
    [Required]
    public TimeOnly To { get; init; }

    /// <summary>
    /// Минимально необходимая вместимость. Вернутся залы, вмещающие не меньше.
    /// </summary>
    /// <example>50</example>
    [DefaultValue(1)]
    [Range(1, 100)]
    public int Capacity { get; init; } = 1;
}
