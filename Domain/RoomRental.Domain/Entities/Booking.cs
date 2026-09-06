using Ardalis.GuardClauses;
using RoomRental.Domain.Enums;
using RoomRental.Domain.Services;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Бронь конференц-зала.
/// </summary>
public class Booking
{
    /// <summary>
    /// Предельная длина причины отмены.
    /// </summary>
    public const int MaxCancellationReasonLength = 100;

    private readonly List<BookingCharge> _charges = [];
    private readonly List<BookingSlot> _slots = [];

    private Booking(
        Guid id,
        Guid roomId,
        BookingPeriod period,
        decimal totalAmount,
        BookingStatus status,
        Guid customerId,
        DateTime createdAtUtc,
        IReadOnlyList<BookingCharge> charges,
        IReadOnlyList<BookingSlot> slots)
    {
        Guard.Against.Default(id);
        Guard.Against.Default(roomId);
        Guard.Against.Null(period);
        Guard.Against.Negative(totalAmount);
        Guard.Against.Default(status);
        Guard.Against.EnumOutOfRange(status);
        Guard.Against.Default(customerId);
        Guard.Against.Default(createdAtUtc);
        Guard.Against.Null(charges);

        Id = id;
        RoomId = roomId;
        Period = period;
        TotalAmount = totalAmount;
        Status = status;
        CustomerId = customerId;
        CreatedAtUtc = createdAtUtc;
        _charges = [.. charges];
        _slots = [.. slots];
    }

    private Booking()
    {
    }

    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Уникальный идентификатор бронируемого конференц-зала.
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Бронируемый конференц-зал.
    /// </summary>
    public Room Room { get; } = null!;

    /// <summary>
    /// Период бронирования: дата и диапазон целых часов.
    /// </summary>
    public BookingPeriod Period { get; } = null!;

    /// <summary>
    /// Итоговая стоимость.
    /// </summary>
    public decimal TotalAmount { get; }

    /// <summary>
    /// Статус.
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Уникальный идентификатор пользователя, совершившего бронь.
    /// </summary>
    public Guid CustomerId { get; }

    /// <summary>
    /// Пользователь, совершивший бронь.
    /// </summary>
    public User Customer { get; } = null!;

    /// <summary>
    /// Дата и время создания брони по UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Дата и время отмены брони по UTC.
    /// </summary>
    public DateTime? CancelledAtUtc { get; private set; }

    /// <summary>
    /// Причина отмены брони.
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    /// Расчёты позиций в счёте.
    /// </summary>
    public IReadOnlyList<BookingCharge> Charges => _charges.AsReadOnly();

    /// <summary>
    /// Часовые слоты бронирования.
    /// </summary>
    public IReadOnlyList<BookingSlot> Slots => _slots.AsReadOnly();

    /// <summary>
    /// Создаёт подтверждённую бронь: считает стоимость по правилам и раскладывает период на часовые слоты.
    /// </summary>
    /// <param name="room">Бронируемый зал.</param>
    /// <param name="period">Период бронирования - одна дата и диапазон целых часов.</param>
    /// <param name="services">Заказанные услуги, каждая должна предоставляться залом.</param>
    /// <param name="pricingRules">Действующие правила ценообразования.</param>
    /// <param name="customerId">Клиент, оформляющий бронь.</param>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    public static Booking Create(
        Room room,
        BookingPeriod period,
        IReadOnlyList<Service> services,
        IReadOnlyList<PricingRule> pricingRules,
        Guid customerId,
        DateTime utcNow)
    {
        Guard.Against.Null(room);
        Guard.Against.Null(period);
        Guard.Against.Null(services);
        Guard.Against.Null(pricingRules);
        Guard.Against.Default(customerId);
        Guard.Against.Default(utcNow);

        if (period.StartsAtUtc < utcNow)
        {
            throw new ArgumentException("Нельзя забронировать зал на прошедшее время.");
        }

        if (room.IsDeleted)
        {
            throw new ArgumentException("Зал удалён.");
        }

        if (services.Any(s => s.IsDeleted))
        {
            throw new ArgumentException("Одна из услуг удалена.");
        }

        var requested = services.DistinctBy(s => s.Id).ToList();

        if (requested.Any(s => room.Services.All(rs => rs.Id != s.Id)))
        {
            throw new ArgumentException("Зал не предоставляет выбранную услугу.");
        }

        var bookingId = Guid.CreateVersion7();

        List<BookingCharge> charges =
        [
            .. RoomTimePricing.Split(bookingId, period, room.BaseHourPrice, pricingRules),
            .. requested.Select(s => BookingCharge.ForService(s, bookingId)),
        ];

        var slots = period.HourlySlots()
            .Select(slot => BookingSlot.Create(bookingId, room.Id, slot))
            .ToList();

        return new Booking(
            bookingId,
            room.Id,
            period,
            totalAmount: charges.Sum(c => c.Amount),
            BookingStatus.Confirmed,
            customerId,
            utcNow,
            charges,
            slots);
    }

    /// <summary>
    /// Отменяет бронь и освобождает занятые часы зала.
    /// Позиции счёта сохраняются - они нужны для истории и отчётов.
    /// Повторная отмена ничего не меняет, начавшуюся бронь отменить нельзя.
    /// </summary>
    /// <param name="utcNow">Текущий момент времени по UTC.</param>
    /// <param name="reason">Причина отмены, необязательна.</param>
    public void Cancel(DateTime utcNow, string? reason = null)
    {
        if (reason?.Length > MaxCancellationReasonLength)
        {
            throw new ArgumentException($"Максимальная длина причины: {MaxCancellationReasonLength}.");
        }

        if (Status == BookingStatus.Cancelled)
        {
            return;
        }

        if (Period.StartsAtUtc <= utcNow)
        {
            throw new InvalidOperationException("Нельзя отменить начавшуюся бронь.");
        }

        Status = BookingStatus.Cancelled;
        CancelledAtUtc = utcNow;
        CancellationReason = reason;
        _slots.Clear();
    }
}