namespace RoomRental.Application.Abstraction.Exceptions;

/// <summary>
/// Пользователь опознан, но не имеет права на этот объект.
/// </summary>
public class AccessDeniedException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public AccessDeniedException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public AccessDeniedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public AccessDeniedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
