using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Users;

/// <summary>
/// Вход по email и паролю.
/// </summary>
public record LoginQuery(string Email, string Password) : IRequest<AccessTokenDto>;
