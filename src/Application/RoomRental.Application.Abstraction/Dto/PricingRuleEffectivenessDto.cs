namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Во что обошлось или что принесло одно правило ценообразования за период.
/// </summary>
/// <param name="PricingRuleId">
/// Уникальный идентификатор правила. Пусто, если часы не покрыты ни одним правилом.
/// </param>
/// <param name="RuleName">Название правила на момент бронирования.</param>
/// <param name="Multiplier">Множитель правила.</param>
/// <param name="Hours">Часов, проданных по этому правилу.</param>
/// <param name="BaseAmount">Сколько стоили бы эти часы по базовой ставке.</param>
/// <param name="ActualAmount">Сколько стоили фактически.</param>
/// <param name="Delta">
/// Разница: отрицательная - отдано скидкой, положительная - заработано наценкой.
/// </param>
public record PricingRuleEffectivenessDto(
    Guid? PricingRuleId,
    string RuleName,
    decimal Multiplier,
    decimal Hours,
    decimal BaseAmount,
    decimal ActualAmount,
    decimal Delta);
