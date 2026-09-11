using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Contracts.Requests.Reports;
using RoomRental.Contracts.Responses;
using RoomRental.Domain.Enums;
using RoomRental.Mapping;

namespace RoomRental.Controllers;

/// <summary>
/// Отчёты для бизнеса. Доступны только администратору.
/// </summary>
[ApiController]
[Route("api/reports")]
[Produces("application/json")]
public class ReportsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Выручка за период с разрезами по залам и услугам.
    /// Отменённые брони не учитываются.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("revenue")]
    [ProducesResponseType<RevenueReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RevenueReportResponse>> GetRevenueAsync(
        [FromQuery] ReportPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var report = await mediator.Send(request.ToRevenueQuery(), cancellationToken);

        return Ok(report.ToResponse());
    }

    /// <summary>
    /// Загрузка залов за период: сколько часов из доступных выкуплено.
    /// За доступные принимаются часы окна бронирования, с 06:00 до 23:00.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("room-utilization")]
    [ProducesResponseType<IReadOnlyList<RoomUtilizationResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<RoomUtilizationResponse>>> GetRoomUtilizationAsync(
        [FromQuery] ReportPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var report = await mediator.Send(request.ToUtilizationQuery(), cancellationToken);

        return Ok(report.ToResponse());
    }

    /// <summary>
    /// Эффективность правил ценообразования: сколько часов продано по каждому правилу
    /// и сколько на нём заработано наценкой или отдано скидкой.
    /// </summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("pricing-rules")]
    [ProducesResponseType<IReadOnlyList<PricingRuleEffectivenessResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<PricingRuleEffectivenessResponse>>> GetPricingRuleEffectivenessAsync(
        [FromQuery] ReportPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var report = await mediator.Send(request.ToPricingRuleQuery(), cancellationToken);

        return Ok(report.ToResponse());
    }
}
