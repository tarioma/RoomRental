using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Выборки для отчётов. Только чтение, доменной логики не содержит.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class ReportRepository(DatabaseContext context) : IReportRepository
{
    private static readonly int BookableHoursPerDay =
        BookingPeriod.LatestEnd.Hour - BookingPeriod.EarliestStart.Hour;

    /// <inheritdoc />
    public async Task<RevenueReportDto> GetRevenueAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default)
    {
        var charges = ConfirmedCharges(period);

        var byKind = await charges
            .GroupBy(c => c.Kind)
            .Select(g => new { Kind = g.Key, Amount = g.Sum(c => c.Amount) })
            .ToListAsync(cancellationToken);

        var byRoom = await ConfirmedBookings(period)
            .GroupBy(b => new { b.RoomId, b.Room.Name })
            .Select(g => new
            {
                g.Key.RoomId,
                g.Key.Name,
                Bookings = g.Count(),
                Revenue = g.Sum(b => b.TotalAmount),
            })
            .ToListAsync(cancellationToken);

        var hoursByRoom = await charges
            .Where(c => c.Kind == ChargeKind.RoomTime)
            .GroupBy(c => c.Booking.RoomId)
            .Select(g => new { RoomId = g.Key, Hours = g.Sum(c => c.Quantity) })
            .ToListAsync(cancellationToken);

        var byService = await charges
            .Where(c => c.Kind == ChargeKind.Service && c.ServiceId != null)
            .GroupBy(c => new { ServiceId = c.ServiceId!.Value, c.Service!.Name })
            .Select(g => new RevenueByServiceDto(
                g.Key.ServiceId,
                g.Key.Name,
                g.Count(),
                g.Sum(c => c.Amount)))
            .ToListAsync(cancellationToken);

        return new RevenueReportDto(
            period.From,
            period.To,
            byKind.Sum(k => k.Amount),
            byKind.Where(k => k.Kind == ChargeKind.RoomTime).Sum(k => k.Amount),
            byKind.Where(k => k.Kind == ChargeKind.Service).Sum(k => k.Amount),
            byRoom.Sum(r => r.Bookings),
            [.. byRoom
                .Select(r => new RevenueByRoomDto(
                    r.RoomId,
                    r.Name,
                    r.Bookings,
                    hoursByRoom.FirstOrDefault(h => h.RoomId == r.RoomId)?.Hours ?? 0m,
                    r.Revenue))
                .OrderByDescending(r => r.Revenue)],
            [.. byService.OrderByDescending(s => s.Revenue)]);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomUtilizationDto>> GetRoomUtilizationAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default)
    {
        var rooms = await context.Rooms
            .AsNoTracking()
            .Where(r => r.DeletedAtUtc == null)
            .Select(r => new { r.Id, r.Name, r.Capacity })
            .ToListAsync(cancellationToken);

        var hoursByRoom = await ConfirmedCharges(period)
            .Where(c => c.Kind == ChargeKind.RoomTime)
            .GroupBy(c => c.Booking.RoomId)
            .Select(g => new { RoomId = g.Key, Hours = g.Sum(c => c.Quantity) })
            .ToListAsync(cancellationToken);

        var revenueByRoom = await ConfirmedBookings(period)
            .GroupBy(b => b.RoomId)
            .Select(g => new { RoomId = g.Key, Revenue = g.Sum(b => b.TotalAmount) })
            .ToListAsync(cancellationToken);

        var availableHours = period.Days * BookableHoursPerDay;

        return
        [
            .. rooms
                .Select(r =>
                {
                    var bookedHours = hoursByRoom.FirstOrDefault(h => h.RoomId == r.Id)?.Hours ?? 0m;

                    return new RoomUtilizationDto(
                        r.Id,
                        r.Name,
                        r.Capacity,
                        bookedHours,
                        availableHours,
                        Math.Round(bookedHours / availableHours * 100m, 1, MidpointRounding.AwayFromZero),
                        revenueByRoom.FirstOrDefault(x => x.RoomId == r.Id)?.Revenue ?? 0m);
                })
                .OrderByDescending(r => r.UtilizationRate)
                .ThenBy(r => r.RoomName),
        ];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PricingRuleEffectivenessDto>> GetPricingRuleEffectivenessAsync(
        ReportPeriod period,
        CancellationToken cancellationToken = default)
    {
        var rows = await ConfirmedCharges(period)
            .Where(c => c.Kind == ChargeKind.RoomTime)
            .GroupBy(c => new { c.PricingRuleId, c.Description, c.Multiplier })
            .Select(g => new
            {
                g.Key.PricingRuleId,
                g.Key.Description,
                g.Key.Multiplier,
                Hours = g.Sum(c => c.Quantity),
                BaseAmount = g.Sum(c => c.Quantity * c.UnitPrice),
                ActualAmount = g.Sum(c => c.Amount),
            })
            .ToListAsync(cancellationToken);

        return
        [
            .. rows
                .Select(r =>
                {
                    var baseAmount = Math.Round(r.BaseAmount, 2, MidpointRounding.AwayFromZero);

                    return new PricingRuleEffectivenessDto(
                        r.PricingRuleId,
                        r.Description,
                        r.Multiplier,
                        r.Hours,
                        baseAmount,
                        r.ActualAmount,
                        r.ActualAmount - baseAmount);
                })
                .OrderByDescending(r => r.Hours),
        ];
    }

    private IQueryable<Booking> ConfirmedBookings(ReportPeriod period) =>
        context.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed
                        && b.Period.Date >= period.From
                        && b.Period.Date <= period.To);

    private IQueryable<BookingCharge> ConfirmedCharges(ReportPeriod period) =>
        context.BookingCharges
            .AsNoTracking()
            .Where(c => c.Booking.Status == BookingStatus.Confirmed
                        && c.Booking.Period.Date >= period.From
                        && c.Booking.Period.Date <= period.To);
}
