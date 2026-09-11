using RoomRental.Application.Abstraction.Commands.Users;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries.Users;
using RoomRental.Contracts.Requests.Users;
using RoomRental.Contracts.Responses;

namespace RoomRental.Mapping;

internal static class UserMapping
{
    public static RegisterUserCommand ToCommand(this RegisterUserRequest request) =>
        new(request.Email, request.Password);

    public static LoginQuery ToQuery(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static AccessTokenResponse ToResponse(this AccessTokenDto dto) =>
        new(dto.Token, dto.ExpiresAtUtc);
}
