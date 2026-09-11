namespace RoomRental.Domain.Enums;

/// <summary>
/// Роль пользователя, определяющая доступные действия.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Значение не задано. Признак незаполненных данных.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Клиент: бронирует залы и распоряжается своими бронями.
    /// </summary>
    Client = 1,

    /// <summary>
    /// Администратор: управляет залами, услугами и видит отчёты.
    /// </summary>
    Admin = 2
}