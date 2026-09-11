using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests;

/// <summary>
/// Постраничная навигация. Верхняя граница защищает от запросов на весь объём данных.
/// </summary>
public record PagedRequest
{
    /// <summary>
    /// Наибольшее допустимое количество записей в одном ответе.
    /// </summary>
    public const int MaxLimit = 100;

    /// <summary>
    /// Сколько записей вернуть, от 1 до 100.
    /// </summary>
    /// <example>20</example>
    [DefaultValue(20)]
    [Range(1, MaxLimit)]
    public int Limit { get; init; } = 20;

    /// <summary>
    /// Сколько записей пропустить от начала выборки.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    [Range(0, int.MaxValue)]
    public int Offset { get; init; }
}
