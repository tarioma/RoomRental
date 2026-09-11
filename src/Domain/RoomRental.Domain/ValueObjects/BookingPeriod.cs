using Ardalis.GuardClauses;

namespace RoomRental.Domain.ValueObjects;

/// <summary>
/// Период бронирования: одна дата и диапазон целых часов внутри рабочего окна.
/// Бронь по определению не выходит за пределы одних суток - это гарантирует сам тип,
/// а не проверка внутри сущности.
/// </summary>
public sealed record BookingPeriod
{
    /// <summary>
    /// Самое раннее время, с которого можно начать бронь.
    /// </summary>
    public static readonly TimeOnly EarliestStart = new(6, 0);

    /// <summary>
    /// Самое позднее время, которым можно закончить бронь.
    /// </summary>
    public static readonly TimeOnly LatestEnd = new(23, 0);

    private BookingPeriod(DateOnly date, TimeOnly from, TimeOnly to)
    {
        Date = date;
        From = from;
        To = to;
    }

    /// <summary>
    /// Дата бронирования.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Время начала.
    /// </summary>
    public TimeOnly From { get; }

    /// <summary>
    /// Время окончания.
    /// </summary>
    public TimeOnly To { get; }

    /// <summary>
    /// Начало периода в UTC.
    /// </summary>
    public DateTime StartsAtUtc => Date.ToDateTime(From, DateTimeKind.Utc);

    /// <summary>
    /// Окончание периода в UTC.
    /// </summary>
    public DateTime EndsAtUtc => Date.ToDateTime(To, DateTimeKind.Utc);

    /// <summary>
    /// Продолжительность периода в целых часах.
    /// </summary>
    public int Hours => To.Hour - From.Hour;

    /// <summary>
    /// Создаёт период бронирования на указанную дату.
    /// </summary>
    /// <param name="date">Дата бронирования.</param>
    /// <param name="from">Время начала, целый час не раньше <see cref="EarliestStart"/>.</param>
    /// <param name="to">Время окончания, целый час не позже <see cref="LatestEnd"/>.</param>
    public static BookingPeriod Create(DateOnly date, TimeOnly from, TimeOnly to)
    {
        Guard.Against.Default(date);

        if (from >= to)
        {
            throw new ArgumentException("Время начала должно быть перед временем окончания.");
        }

        if (from < EarliestStart || to > LatestEnd)
        {
            throw new ArgumentException(
                $"Бронирование возможно только с {EarliestStart:HH\\:mm} до {LatestEnd:HH\\:mm}.");
        }

        if (from.Ticks % TimeSpan.TicksPerHour != 0 || to.Ticks % TimeSpan.TicksPerHour != 0)
        {
            throw new ArgumentException("Бронирование возможно только по целым часам.");
        }

        return new BookingPeriod(date, from, to);
    }

    /// <summary>
    /// Начала часовых слотов периода - по одному на каждый час.
    /// </summary>
    public IEnumerable<DateTime> HourlySlots()
    {
        for (var slot = StartsAtUtc; slot < EndsAtUtc; slot = slot.AddHours(1))
        {
            yield return slot;
        }
    }

    /// <inheritdoc />
    public override string ToString() => $@"{Date:yyyy-MM-dd} {From:HH\:mm}-{To:HH\:mm}";
}
