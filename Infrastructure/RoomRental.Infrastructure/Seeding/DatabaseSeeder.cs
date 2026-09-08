using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoomRental.Application.Abstraction.Security;
using RoomRental.Dal;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;

namespace RoomRental.Infrastructure.Seeding;

/// <summary>
/// Наполняет базу начальными данными из технического задания.
/// Каждый набор проверяется отдельно, поэтому повторный запуск ничего не дублирует,
/// а частично заполненная база достраивается.
/// </summary>
public class DatabaseSeeder(
    DatabaseContext context,
    IPasswordHasher passwordHasher,
    IOptions<SeedOptions> options,
    TimeProvider timeProvider,
    ILogger<DatabaseSeeder> logger)
{
    /// <summary>
    /// Наполняет базу начальными данными, если их ещё нет.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var services = await EnsureServicesAsync(utcNow, cancellationToken);

        await EnsureRoomsAsync(services, utcNow, cancellationToken);
        await EnsurePricingRulesAsync(cancellationToken);
        await EnsureAdminAsync(utcNow, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<Service>> EnsureServicesAsync(
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        if (await context.Services.AnyAsync(cancellationToken))
        {
            return await context.Services.ToListAsync(cancellationToken);
        }

        List<Service> services =
        [
            Service.Create("Проектор", 500m, utcNow),
            Service.Create("Wi-Fi", 300m, utcNow),
            Service.Create("Звук", 700m, utcNow),
        ];

        context.Services.AddRange(services);
        logger.LogInformation("Добавлены услуги: {Count}", services.Count);

        return services;
    }

    private async Task EnsureRoomsAsync(
        IReadOnlyList<Service> services,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        if (await context.Rooms.AnyAsync(cancellationToken))
        {
            return;
        }

        context.Rooms.AddRange(
            Room.Create("Зал А", 50, 2000m, services, utcNow),
            Room.Create("Зал B", 100, 3500m, services, utcNow),
            Room.Create("Зал C", 30, 1500m, services, utcNow));

        logger.LogInformation("Добавлены залы: 3");
    }

    private async Task EnsurePricingRulesAsync(CancellationToken cancellationToken)
    {
        if (await context.PricingRules.AnyAsync(cancellationToken))
        {
            return;
        }

        // Пиковые часы лежат внутри стандартных, поэтому их приоритет выше.
        context.PricingRules.AddRange(
            PricingRule.Create("Утренние часы", new TimeOnly(6, 0), new TimeOnly(9, 0), 0.90m, 10),
            PricingRule.Create("Стандартные часы", new TimeOnly(9, 0), new TimeOnly(18, 0), 1.00m, 10),
            PricingRule.Create("Пиковые часы", new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m, 20),
            PricingRule.Create("Вечерние часы", new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m, 10));

        logger.LogInformation("Добавлены правила ценообразования: 4");
    }

    private async Task EnsureAdminAsync(DateTime utcNow, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(options.Value.AdminEmail);

        if (await context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken))
        {
            return;
        }

        context.Users.Add(User.Create(
            options.Value.AdminEmail,
            passwordHasher.Hash(options.Value.AdminPassword),
            UserRole.Admin,
            utcNow));

        logger.LogInformation("Добавлен администратор {Email}", options.Value.AdminEmail);
    }
}
