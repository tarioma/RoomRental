namespace RoomRental.Contracts.Responses;

/// <summary>
/// Что правило ценообразования принесло или стоило за период.
/// Позволяет понять, окупаются ли скидки и оправдана ли наценка.
/// </summary>
/// <param name="PricingRuleId">
/// Уникальный идентификатор правила. Пусто, если часы не покрыты ни одним правилом
/// и тарифицировались по базовой ставке.
/// </param>
/// <param name="RuleName">Название правила на момент бронирования.</param>
/// <param name="Multiplier">Множитель правила: 1.15 - наценка 15%, 0.8 - скидка 20%.</param>
/// <param name="Hours">Сколько часов продано по этому правилу.</param>
/// <param name="BaseAmount">Сколько эти часы стоили бы по базовой ставке, без множителя.</param>
/// <param name="ActualAmount">Сколько они стоили фактически.</param>
/// <param name="Delta">
/// Разница между фактом и базой: отрицательная - столько отдано скидкой,
/// положительная - столько заработано наценкой.
/// </param>
public record PricingRuleEffectivenessResponse(
    Guid? PricingRuleId,
    string RuleName,
    decimal Multiplier,
    decimal Hours,
    decimal BaseAmount,
    decimal ActualAmount,
    decimal Delta);
