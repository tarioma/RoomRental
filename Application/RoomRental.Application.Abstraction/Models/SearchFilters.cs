namespace RoomRental.Application.Abstraction.Models;

/// <summary>
/// Постраничная навигация по выборке.
/// </summary>
/// <param name="Limit">Сколько записей вернуть.</param>
/// <param name="Offset">Сколько записей пропустить от начала выборки.</param>
public record SearchFilters(int Limit, int Offset);
