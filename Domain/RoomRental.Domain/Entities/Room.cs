using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Конференц-зал.
/// </summary>
public class Room
{
    /// <summary>
    /// Предельная длина названия зала.
    /// </summary>
    public const int MaxNameLength = 100;
    /// <summary>
    /// Предельная вместимость зала.
    /// </summary>
    public const int MaxCapacity = 100;

    private readonly List<Service> _services = [];

    private Room(
        Guid id,
        string name,
        int capacity,
        decimal baseHourPrice,
        IReadOnlyList<Service> services,
        DateTime createdAtUtc)
    {
        Guard.Against.Default(id);
        Guard.Against.Default(createdAtUtc);

        Id = id;
        SetName(name);
        SetCapacity(capacity);
        SetBaseHourPrice(baseHourPrice);
        CreatedAtUtc = createdAtUtc;
        SetServices(services);
    }

    private Room()
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
    /// Вместимость.
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    /// Базовая цена аренды в час.
    /// </summary>
    public decimal BaseHourPrice { get; private set; }

    /// <summary>
    /// Дата и время создания по UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Дата и время удаления по UTC.
    /// </summary>
    public DateTime? DeletedAtUtc { get; private set; }

    /// <summary>
    /// Удалён ли конференц-зал.
    /// </summary>
    public bool IsDeleted => DeletedAtUtc.HasValue;

    /// <summary>
    /// Доступные для зала услуги.
    /// </summary>
    public IReadOnlyList<Service> Services => _services.AsReadOnly();

    /// <summary>
    /// Создаёт зал с набором доступных в нём услуг.
    /// </summary>
    /// <param name="name">Название.</param>
    /// <param name="capacity">Вместимость в людях.</param>
    /// <param name="baseHourPrice">Базовая стоимость аренды за час.</param>
    /// <param name="services">Услуги, которые можно заказать при бронировании этого зала.</param>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public static Room Create(
        string name,
        int capacity,
        decimal baseHourPrice,
        IReadOnlyList<Service> services,
        DateTime utcNow)
    {
        return new Room(
            Guid.CreateVersion7(),
            name,
            capacity,
            baseHourPrice,
            services,
            utcNow);
    }

    /// <summary>
    /// Меняет название зала.
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
    /// Меняет вместимость зала.
    /// </summary>
    /// <param name="capacity">Новая вместимость.</param>
    public void SetCapacity(int capacity)
    {
        Guard.Against.OutOfRange(capacity, nameof(capacity), 1, MaxCapacity);

        Capacity = capacity;
    }

    /// <summary>
    /// Меняет базовую стоимость аренды за час.
    /// </summary>
    /// <param name="baseHourPrice">Новая стоимость.</param>
    public void SetBaseHourPrice(decimal baseHourPrice)
    {
        Guard.Against.NegativeOrZero(baseHourPrice);

        BaseHourPrice = baseHourPrice;
    }

    /// <summary>
    /// Добавляет услугу в набор доступных. Повторное добавление ничего не меняет.
    /// </summary>
    /// <param name="service">Добавляемая услуга.</param>
    public void AddService(Service service)
    {
        Guard.Against.Null(service);

        if (service.IsDeleted)
        {
            throw new ArgumentException("Нельзя добавить удалённую услугу.");
        }

        if (_services.Exists(s => s.Id == service.Id))
        {
            return;
        }

        _services.Add(service);
    }

    /// <summary>
    /// Исключает услугу из набора доступных.
    /// </summary>
    /// <param name="serviceId">Исключаемая услуга.</param>
    public void RemoveService(Guid serviceId) => _services.RemoveAll(s => s.Id == serviceId);

    /// <summary>
    /// Мягко удаляет зал. Повторный вызов ничего не меняет.
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

    private void SetServices(IReadOnlyList<Service> services)
    {
        Guard.Against.Null(services);

        if (services.Any(s => s.IsDeleted))
        {
            throw new ArgumentException("Нельзя добавить удаленную услугу.");
        }

        var target = services.DistinctBy(s => s.Id).ToList();

        _services.RemoveAll(s => !target.Exists(t => t.Id == s.Id));

        foreach (var service in target.Where(t => !_services.Exists(s => s.Id == t.Id)))
        {
            _services.Add(service);
        }
    }
}