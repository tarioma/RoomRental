namespace RoomRental.Contracts.Responses;

/// <summary>
/// Выручка за период. Отменённые брони не учитываются.
/// </summary>
/// <param name="From">Начало периода включительно.</param>
/// <param name="To">Окончание периода включительно.</param>
/// <param name="Total">Итоговая выручка - аренда плюс услуги.</param>
/// <param name="RoomTimeTotal">Выручка только от аренды залов.</param>
/// <param name="ServicesTotal">Выручка только от дополнительных услуг.</param>
/// <param name="Bookings">Количество подтверждённых броней за период.</param>
/// <param name="ByRoom">Разрез по залам, от большей выручки к меньшей.</param>
/// <param name="ByService">Разрез по услугам, от большей выручки к меньшей.</param>
public record RevenueReportResponse(
    DateOnly From,
    DateOnly To,
    decimal Total,
    decimal RoomTimeTotal,
    decimal ServicesTotal,
    int Bookings,
    IReadOnlyList<RevenueByRoomResponse> ByRoom,
    IReadOnlyList<RevenueByServiceResponse> ByService);
