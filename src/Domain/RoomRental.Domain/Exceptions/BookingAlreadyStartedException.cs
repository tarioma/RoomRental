namespace RoomRental.Domain.Exceptions;

/// <summary>
/// Бронь уже началась, поэтому изменить её нельзя.
/// </summary>
public class BookingAlreadyStartedException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public BookingAlreadyStartedException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public BookingAlreadyStartedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public BookingAlreadyStartedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
