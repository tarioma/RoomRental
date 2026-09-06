using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Правило бронирования в определённое время суток.
/// Например, в пиковые часы (с 12:00 до 14:00) стоимость аренды может быть с наценкой в 15%.
/// </summary>
public class PricingRule
{
    /// <summary>
    /// Предельная длина названия правила.
    /// </summary>
    public const int MaxNameLength = 100;

    private PricingRule(
        Guid id,
        string name,
        TimeOnly from,
        TimeOnly to,
        decimal multiplier,
        int priority,
        bool isActive)
    {
        Guard.Against.Default(id);
        
        Id = id;
        SetName(name);
        SetPeriod(from, to);
        SetMultiplier(multiplier);
        Priority = priority;
        IsActive = isActive;
    }

    private PricingRule()
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
    /// Время, с которого действует правило.
    /// </summary>
    public TimeOnly From { get; private set; }

    /// <summary>
    /// Время, до которого действует правило.
    /// </summary>
    public TimeOnly To { get; private set; }

    /// <summary>
    /// Множитель.
    /// Например: +15% = 1.15, -20% = 0.8.
    /// </summary>
    public decimal Multiplier { get; private set; }

    /// <summary>
    /// Приоритет правила над другими правилами. Побеждает правило с большим приоритетом.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Актуально ли правило на данный момент.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Создаёт действующее правило ценообразования.
    /// </summary>
    /// <param name="name">Название, например «Пиковые часы».</param>
    /// <param name="from">Время начала действия.</param>
    /// <param name="to">Время окончания действия.</param>
    /// <param name="multiplier">Множитель: 1.15 - наценка 15%, 0.8 - скидка 20%.</param>
    /// <param name="priority">Приоритет при пересечении с другими правилами.</param>
    public static PricingRule Create(
        string name,
        TimeOnly from,
        TimeOnly to,
        decimal multiplier,
        int priority)
    {
        return new PricingRule(
            Guid.CreateVersion7(),
            name,
            from,
            to,
            multiplier,
            priority,
            true);
    }
    
    /// <summary>
    /// Меняет название правила.
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
    /// Меняет период действия правила. Граница начала включается, граница окончания - нет.
    /// </summary>
    /// <param name="from">Время начала.</param>
    /// <param name="to">Время окончания.</param>
    public void SetPeriod(TimeOnly from, TimeOnly to)
    {
        if (from >= to)
        {
            throw new ArgumentException("Начало периода должно быть раньше окончания.");
        }

        From = from;
        To = to;
    }

    /// <summary>
    /// Меняет множитель правила.
    /// </summary>
    /// <param name="multiplier">Новый множитель.</param>
    public void SetMultiplier(decimal multiplier)
    {
        Guard.Against.NegativeOrZero(multiplier);
        
        Multiplier = multiplier;
    }
}