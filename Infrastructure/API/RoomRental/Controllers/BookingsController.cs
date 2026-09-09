using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Application.Abstraction.Queries.Bookings;
using RoomRental.Contracts.Requests.Bookings;
using RoomRental.Contracts.Responses;
using RoomRental.Extensions;
using RoomRental.Mapping;

namespace RoomRental.Controllers;

/// <summary>
/// Бронирование залов.
/// </summary>
[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
public class BookingsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Возвращает бронь с полной расшифровкой расчёта стоимости.
    /// Доступна владельцу брони и администратору.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var booking = await mediator.Send(new GetBookingByIdQuery(id, User.GetUserId(), User.GetUserRole()), cancellationToken);

        return Ok(booking.ToResponse());
    }

    /// <summary>
    /// Бронирует зал на указанный период и возвращает подтверждение с расчётом стоимости.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> CreateAsync(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await mediator.Send(request.ToCommand(User.GetUserId()), cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = booking.Id }, booking.ToResponse());
    }

    /// <summary>
    /// Отменяет бронь. Доступно владельцу брони и администратору.
    /// Начавшуюся бронь отменить нельзя.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CancelAsync(
        [FromRoute] Guid id,
        [FromBody] CancelBookingRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(id, User.GetUserId(), User.GetUserRole()), cancellationToken);

        return NoContent();
    }
}
