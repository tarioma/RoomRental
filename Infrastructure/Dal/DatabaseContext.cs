using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal;

/// <summary>
/// Контекст базы данных. Настройки сущностей подхватываются из этой же сборки.
/// Он же выступает единицей работы: <see cref="IUnitOfWork.SaveChangesAsync"/> -
/// это собственный метод контекста, отдельная обёртка не нужна.
/// </summary>
/// <param name="options">Параметры подключения и поставщика данных.</param>
public class DatabaseContext(DbContextOptions<DatabaseContext> options)
    : DbContext(options), IUnitOfWork
{
    /// <summary>
    /// Конференц-залы.
    /// </summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>
    /// Услуги, доступные при бронировании.
    /// </summary>
    public DbSet<Service> Services => Set<Service>();

    /// <summary>
    /// Брони залов.
    /// </summary>
    public DbSet<Booking> Bookings => Set<Booking>();

    /// <summary>
    /// Позиции счёта: отрезки аренды и заказанные услуги.
    /// </summary>
    public DbSet<BookingCharge> BookingCharges => Set<BookingCharge>();

    /// <summary>
    /// Часовые слоты броней. Уникальность пары зала и часа не даёт забронировать одно время дважды.
    /// </summary>
    public DbSet<BookingSlot> BookingSlots => Set<BookingSlot>();

    /// <summary>
    /// Правила ценообразования по времени суток.
    /// </summary>
    public DbSet<PricingRule> PricingRules => Set<PricingRule>();

    /// <summary>
    /// Пользователи.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
