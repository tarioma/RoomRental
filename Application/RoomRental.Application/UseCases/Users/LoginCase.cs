using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Queries.Users;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Abstraction.Security;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.UseCases.Users;

/// <summary>
/// Проверяет пару email и пароль и выдаёт токен доступа.
/// </summary>
public class LoginCase(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IAccessTokenGenerator accessTokenGenerator)
    : IRequestHandler<LoginQuery, AccessTokenDto>
{
    /// <inheritdoc />
    public async Task<AccessTokenDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Email);
        Guard.Against.NullOrWhiteSpace(request.Password);

        var user = await users.GetByNormalizedEmailAsync(
            User.NormalizeEmail(request.Email),
            cancellationToken);

        // Причина отказа наружу не уточняется: иначе перебором можно узнать,
        // какие адреса зарегистрированы.
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException("Неверный email или пароль.");
        }

        return accessTokenGenerator.Generate(user);
    }
}
