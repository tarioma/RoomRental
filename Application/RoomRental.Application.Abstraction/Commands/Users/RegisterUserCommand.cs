using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Users;

/// <summary>
/// Регистрация клиента.
/// </summary>
/// <param name="Email">Email-адрес, уникальный без учёта регистра.</param>
/// <param name="Password">Пароль в открытом виде, наружу нигде не сохраняется.</param>
public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;
