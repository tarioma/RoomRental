namespace RoomRental.Application.Abstraction.Exceptions;

/// <summary>
/// Пользователь с таким email уже зарегистрирован.
/// </summary>
public class EmailAlreadyTakenException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public EmailAlreadyTakenException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public EmailAlreadyTakenException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public EmailAlreadyTakenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
