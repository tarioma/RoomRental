using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries.Reports;
using RoomRental.Contracts.Requests.Reports;
using RoomRental.Contracts.Responses;

namespace RoomRental.Mapping;

internal static class ReportMapping
{
    public static GetRevenueReportQuery ToRevenueQuery(this ReportPeriodRequest request) =>
        new(request.From, request.To);

    public static GetRoomUtilizationReportQuery ToUtilizationQuery(this ReportPeriodRequest request) =>
        new(request.From, request.To);

    public static GetPricingRuleEffectivenessReportQuery ToPricingRuleQuery(this ReportPeriodRequest request) =>
        new(request.From, request.To);

    public static RevenueReportResponse ToResponse(this RevenueReportDto dto) =>
        new(
            dto.From,
            dto.To,
            dto.Total,
            dto.RoomTimeTotal,
            dto.ServicesTotal,
            dto.Bookings,
            [.. dto.ByRoom.Select(r => new RevenueByRoomResponse(
                r.RoomId, r.RoomName, r.Bookings, r.Hours, r.Revenue))],
            [.. dto.ByService.Select(s => new RevenueByServiceResponse(
                s.ServiceId, s.ServiceName, s.Orders, s.Revenue))]);

    public static IReadOnlyList<RoomUtilizationResponse> ToResponse(this IReadOnlyList<RoomUtilizationDto> dtos) =>
        [.. dtos.Select(d => new RoomUtilizationResponse(
            d.RoomId,
            d.RoomName,
            d.Capacity,
            d.BookedHours,
            d.AvailableHours,
            d.UtilizationRate,
            d.Revenue))];

    public static IReadOnlyList<PricingRuleEffectivenessResponse> ToResponse(
        this IReadOnlyList<PricingRuleEffectivenessDto> dtos) =>
        [.. dtos.Select(d => new PricingRuleEffectivenessResponse(
            d.PricingRuleId,
            d.RuleName,
            d.Multiplier,
            d.Hours,
            d.BaseAmount,
            d.ActualAmount,
            d.Delta))];
}
