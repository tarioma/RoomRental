namespace RoomRental.Application.Abstraction.Exceptions;

/// <summary>
/// Зал уже занят на запрошенные часы.
/// </summary>
public class SlotAlreadyBookedException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public SlotAlreadyBookedException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public SlotAlreadyBookedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public SlotAlreadyBookedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
