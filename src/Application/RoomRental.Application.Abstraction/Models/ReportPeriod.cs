using Ardalis.GuardClauses;

namespace RoomRental.Application.Abstraction.Models;

/// <summary>
/// Диапазон дат, за который строится отчёт. Обе границы включаются.
/// </summary>
public record ReportPeriod
{
    /// <summary>
    /// Санитарная граница длины периода: отсекает заведомо бессмысленные диапазоны
    /// вроде опечатки в годе. Нагрузку она не ограничивает - за это отвечает
    /// индекс по дате брони.
    /// </summary>
    public const int MaxDays = 3653;

    private ReportPeriod(DateOnly from, DateOnly to)
    {
        From = from;
        To = to;
    }

    /// <summary>
    /// Начало периода включительно.
    /// </summary>
    public DateOnly From { get; }

    /// <summary>
    /// Окончание периода включительно.
    /// </summary>
    public DateOnly To { get; }

    /// <summary>
    /// Количество суток в периоде, включая обе границы.
    /// </summary>
    public int Days => To.DayNumber - From.DayNumber + 1;

    /// <summary>
    /// Создаёт период отчёта.
    /// </summary>
    /// <param name="from">Начало включительно.</param>
    /// <param name="to">Окончание включительно.</param>
    /// <exception cref="ArgumentException">Границы перепутаны или период слишком длинный.</exception>
    public static ReportPeriod Create(DateOnly from, DateOnly to)
    {
        Guard.Against.Default(from);
        Guard.Against.Default(to);

        if (from > to)
        {
            throw new ArgumentException("Начало периода должно быть не позже окончания.");
        }

        if (to.DayNumber - from.DayNumber + 1 > MaxDays)
        {
            throw new ArgumentException($"Период отчёта не может превышать {MaxDays} дней.");
        }

        return new ReportPeriod(from, to);
    }
}
