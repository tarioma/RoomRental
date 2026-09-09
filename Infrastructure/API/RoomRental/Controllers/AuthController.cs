using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Contracts.Requests.Users;
using RoomRental.Contracts.Responses;
using RoomRental.Mapping;

namespace RoomRental.Controllers;

/// <summary>
/// Регистрация и вход.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>Регистрирует клиента.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<CreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatedResponse>> RegisterAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request.ToCommand(), cancellationToken);

        return Created(string.Empty, new CreatedResponse(id));
    }

    /// <summary>Выдаёт токен доступа по email и паролю.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var token = await mediator.Send(request.ToQuery(), cancellationToken);

        return Ok(token.ToResponse());
    }
}
