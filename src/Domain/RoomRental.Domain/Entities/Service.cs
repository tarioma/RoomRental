using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Услуга, доступная при бронировании конференц-зала.
/// </summary>
public class Service
{
    /// <summary>
    /// Предельная длина названия услуги.
    /// </summary>
    public const int MaxNameLength = 100;

    private Service(
        Guid id,
        string name,
        decimal price,
        DateTime createdAtUtc)
    {
        Guard.Against.Default(id);
        Guard.Against.Default(createdAtUtc);

        Id = id;
        SetName(name);
        SetPrice(price);
        CreatedAtUtc = createdAtUtc;
    }

    private Service()
    {
    }

    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Цена.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Дата и время создания по UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Дата и время удаления по UTC.
    /// </summary>
    public DateTime? DeletedAtUtc { get; private set; }

    /// <summary>
    /// Удалена ли услуга.
    /// </summary>
    public bool IsDeleted => DeletedAtUtc.HasValue;
    
    /// <summary>
    /// Создаёт услугу.
    /// </summary>
    /// <param name="name">Название.</param>
    /// <param name="price">Стоимость за одно бронирование.</param>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public static Service Create(string name, decimal price, DateTime utcNow) => new(
        Guid.CreateVersion7(),
        name,
        price,
        utcNow);

    /// <summary>
    /// Меняет название услуги.
    /// </summary>
    /// <param name="name">Новое название.</param>
    [MemberNotNull(nameof(Name))]
    public void SetName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        
        var trimmed = name.Trim();
        Guard.Against.StringTooLong(trimmed, MaxNameLength);

        Name = trimmed;
    }

    /// <summary>
    /// Меняет стоимость услуги. На уже созданные брони не влияет.
    /// </summary>
    /// <param name="price">Новая стоимость.</param>
    public void SetPrice(decimal price)
    {
        Guard.Against.Negative(price);

        Price = price;
    }

    /// <summary>
    /// Мягко удаляет услугу. Повторный вызов ничего не меняет.
    /// </summary>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public void Delete(DateTime utcNow)
    {
        if (IsDeleted)
        {
            return;
        }

        DeletedAtUtc = utcNow;
    }
}