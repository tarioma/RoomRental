namespace RoomRental.Application.Abstraction.Exceptions;

/// <summary>
/// Запрошенная сущность не найдена или удалена.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// Создаёт исключение без описания.
    /// </summary>
    public EntityNotFoundException()
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение с описанием причины и исходной ошибкой.
    /// </summary>
    /// <param name="message">Текст, который увидит клиент API.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
