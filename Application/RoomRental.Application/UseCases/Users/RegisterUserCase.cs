using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Users;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Abstraction.Security;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;

namespace RoomRental.Application.UseCases.Users;

/// <summary>
/// Регистрирует нового клиента. Пароль сохраняется только в виде хеша.
/// </summary>
public class RegisterUserCase(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<RegisterUserCommand, Guid>
{
    private const int MinPasswordLength = 8;

    /// <inheritdoc />
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Password);

        if (request.Password.Length < MinPasswordLength)
        {
            throw new ArgumentException($"Минимальная длина пароля: {MinPasswordLength}.");
        }

        if (await users.ExistsByNormalizedEmailAsync(User.NormalizeEmail(request.Email), cancellationToken))
        {
            throw new EmailAlreadyTakenException("Пользователь с таким email уже зарегистрирован.");
        }

        var user = User.Create(
            request.Email,
            passwordHasher.Hash(request.Password),
            UserRole.Client,
            timeProvider.GetUtcNow().UtcDateTime);

        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
