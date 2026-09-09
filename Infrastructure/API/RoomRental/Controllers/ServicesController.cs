using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Application.Abstraction.Commands.Services;
using RoomRental.Application.Abstraction.Queries.Services;
using RoomRental.Contracts.Requests.Services;
using RoomRental.Contracts.Responses;
using RoomRental.Domain.Enums;
using RoomRental.Mapping;

namespace RoomRental.Controllers;

/// <summary>
/// Услуги, доступные при бронировании залов.
/// </summary>
[ApiController]
[Route("api/services")]
[Produces("application/json")]
public class ServicesController(IMediator mediator) : ControllerBase
{
    /// <summary>Возвращает услугу по идентификатору.</summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ServiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var service = await mediator.Send(new GetServiceByIdQuery(id), cancellationToken);

        return service is null ? NotFound() : Ok(service.ToResponse());
    }

    /// <summary>Возвращает список услуг постранично.</summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ServiceResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceResponse>>> SearchAsync(
        [FromQuery] SearchServicesRequest request,
        CancellationToken cancellationToken)
    {
        var services = await mediator.Send(request.ToQuery(), cancellationToken);

        return Ok(services.ToResponse());
    }

    /// <summary>Создаёт услугу.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ProducesResponseType<CreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> CreateAsync(
        [FromBody] CreateServiceRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request.ToCommand(), cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id }, new CreatedResponse(id));
    }

    /// <summary>Обновляет название и цену услуги.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateServiceRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Удаляет услугу.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteServiceCommand(id), cancellationToken);

        return NoContent();
    }
}
