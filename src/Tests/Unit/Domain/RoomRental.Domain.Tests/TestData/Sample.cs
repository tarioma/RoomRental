using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Domain.Tests.TestData;

/// <summary>
/// Начальные данные из ТЗ и фабрики для тестов.
/// Время зафиксировано, чтобы результаты не зависели от реальных часов.
/// </summary>
internal static class Sample
{
    /// <summary>
    /// Момент «сейчас» во всех тестах - полночь фиксированного дня в будущем.
    /// Бронь нельзя оформить на прошедшее время, поэтому опорная точка вынесена вперёд.
    /// </summary>
    public static readonly DateTime UtcNow = new(2030, 6, 10, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Идентификатор клиента, от имени которого создаются брони.
    /// </summary>
    public static readonly Guid CustomerId = Guid.CreateVersion7();

    /// <summary>
    /// Базовая стоимость аренды зала А из ТЗ.
    /// </summary>
    public const decimal BaseHourPrice = 2000m;

    /// <summary>
    /// Дата, смещённая на <paramref name="dayOffset"/> суток от <see cref="UtcNow"/>.
    /// </summary>
    public static DateOnly Day(int dayOffset = 0) => DateOnly.FromDateTime(UtcNow.AddDays(dayOffset));

    /// <summary>
    /// Момент времени в UTC - для проверки часовых слотов и границ сегментов.
    /// </summary>
    public static DateTime At(int hour, int dayOffset = 0) => UtcNow.AddDays(dayOffset).AddHours(hour);

    /// <summary>
    /// Период бронирования на указанный диапазон целых часов.
    /// </summary>
    public static BookingPeriod Period(int fromHour, int toHour, int dayOffset = 0) =>
        BookingPeriod.Create(Day(dayOffset), new TimeOnly(fromHour, 0), new TimeOnly(toHour, 0));

    /// <summary>
    /// Правила ценообразования из ТЗ.
    /// Пиковые часы лежат внутри стандартных, поэтому их приоритет выше.
    /// </summary>
    public static List<PricingRule> PricingRules() =>
    [
        PricingRule.Create("Утренние часы", new TimeOnly(6, 0), new TimeOnly(9, 0), 0.90m, 10),
        PricingRule.Create("Стандартные часы", new TimeOnly(9, 0), new TimeOnly(18, 0), 1.00m, 10),
        PricingRule.Create("Пиковые часы", new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m, 20),
        PricingRule.Create("Вечерние часы", new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m, 10),
    ];

    /// <summary>
    /// Проектор из ТЗ - 500 гривен.
    /// </summary>
    public static Service Projector() => Service.Create("Проектор", 500m, UtcNow);

    /// <summary>
    /// Wi-Fi из ТЗ - 300 гривен.
    /// </summary>
    public static Service Wifi() => Service.Create("Wi-Fi", 300m, UtcNow);

    /// <summary>
    /// Зал А из ТЗ с перечисленными доступными услугами.
    /// </summary>
    public static Room RoomA(params Service[] services) =>
        Room.Create("Зал А", 50, BaseHourPrice, services, UtcNow);

    /// <summary>
    /// Бронь зала А на указанный диапазон часов.
    /// </summary>
    public static Booking Book(int fromHour, int toHour, Room? room = null, params Service[] services) =>
        Booking.Create(
            room ?? RoomA(services),
            Period(fromHour, toHour),
            services,
            PricingRules(),
            CustomerId,
            UtcNow);
}
