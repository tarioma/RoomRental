using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Queries.Rooms;
using RoomRental.Contracts.Requests.Rooms;
using RoomRental.Contracts.Responses;
using RoomRental.Domain.Enums;
using RoomRental.Mapping;

namespace RoomRental.Controllers;

/// <summary>
/// Конференц-залы: справочник, подбор свободных и управление составом услуг.
/// </summary>
[ApiController]
[Route("api/rooms")]
[Produces("application/json")]
public class RoomsController(IMediator mediator) : ControllerBase
{
    /// <summary>Возвращает зал вместе с доступными в нём услугами.</summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<RoomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var room = await mediator.Send(new GetRoomByIdQuery(id), cancellationToken);

        return Ok(room.ToResponse());
    }

    /// <summary>Возвращает список залов постранично.</summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RoomResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> SearchAsync(
        [FromQuery] SearchRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var rooms = await mediator.Send(request.ToQuery(), cancellationToken);

        return Ok(rooms.ToResponse());
    }

    /// <summary>
    /// Подбирает залы, свободные на весь указанный период и подходящие по вместимости.
    /// Бронировать можно с 06:00 до 23:00 по целым часам в пределах одних суток.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("available")]
    [ProducesResponseType<IReadOnlyList<RoomResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> SearchAvailableAsync(
        [FromQuery] SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var rooms = await mediator.Send(request.ToQuery(), cancellationToken);

        return Ok(rooms.ToResponse());
    }

    /// <summary>Создаёт зал с набором доступных услуг.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ProducesResponseType<CreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> CreateAsync(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request.ToCommand(), cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id }, new CreatedResponse(id));
    }

    /// <summary>Обновляет название, вместимость и стоимость аренды зала.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Удаляет зал.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteRoomCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Добавляет услугу в набор доступных для зала.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost("{id:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddServiceAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddServiceToRoomCommand(id, serviceId), cancellationToken);

        return NoContent();
    }

    /// <summary>Убирает услугу из набора доступных для зала.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveServiceAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveServiceFromRoomCommand(id, serviceId), cancellationToken);

        return NoContent();
    }
}
