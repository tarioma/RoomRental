namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Выручка за период. Учитываются только подтверждённые брони.
/// </summary>
/// <param name="From">Начало периода.</param>
/// <param name="To">Окончание периода.</param>
/// <param name="Total">Итоговая выручка.</param>
/// <param name="RoomTimeTotal">Выручка от аренды залов.</param>
/// <param name="ServicesTotal">Выручка от дополнительных услуг.</param>
/// <param name="Bookings">Количество подтверждённых броней.</param>
/// <param name="ByRoom">Разрез по залам.</param>
/// <param name="ByService">Разрез по услугам.</param>
public record RevenueReportDto(
    DateOnly From,
    DateOnly To,
    decimal Total,
    decimal RoomTimeTotal,
    decimal ServicesTotal,
    int Bookings,
    IReadOnlyList<RevenueByRoomDto> ByRoom,
    IReadOnlyList<RevenueByServiceDto> ByService);
