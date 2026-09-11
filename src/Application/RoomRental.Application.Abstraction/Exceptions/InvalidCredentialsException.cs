namespace RoomRental.Application.Abstraction.Exceptions;

/// <summary>
/// Пара email и пароль не подошла.
/// </summary>
public class InvalidCredentialsException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public InvalidCredentialsException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public InvalidCredentialsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public InvalidCredentialsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
