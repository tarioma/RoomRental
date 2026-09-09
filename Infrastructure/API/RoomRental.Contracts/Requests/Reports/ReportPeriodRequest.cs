using System.ComponentModel.DataAnnotations;

namespace RoomRental.Contracts.Requests.Reports;

/// <summary>
/// Диапазон дат отчёта. Обе границы включаются.
/// </summary>
public record ReportPeriodRequest : IValidatableObject
{
    /// <summary>
    /// Санитарная граница длины периода - примерно десять лет.
    /// </summary>
    public const int MaxDays = 3653;

    /// <summary>
    /// Начало периода включительно.
    /// </summary>
    /// <example>2026-01-01</example>
    [Required]
    public DateOnly From { get; init; }

    /// <summary>
    /// Окончание периода включительно.
    /// </summary>
    /// <example>2026-12-31</example>
    [Required]
    public DateOnly To { get; init; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        string[] members = [nameof(From), nameof(To)];

        if (From > To)
        {
            yield return new ValidationResult("Начало периода должно быть не позже окончания.", members);
        }
        else if (To.DayNumber - From.DayNumber + 1 > MaxDays)
        {
            yield return new ValidationResult($"Период отчёта не может превышать {MaxDays} дней.", members);
        }
    }
}
