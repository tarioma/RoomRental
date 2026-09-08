using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Security;
using RoomRental.Domain.Entities;

namespace RoomRental.Infrastructure.Security;

/// <summary>
/// Выпуск подписанных JWT-токенов доступа.
/// </summary>
/// <param name="options">Настройки.</param>
/// <param name="timeProvider">Источник текущего времени.</param>
public class JwtAccessTokenGenerator(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider)
    : IAccessTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    /// <inheritdoc />
    public AccessTokenDto Generate(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var issuedAt = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = issuedAt.AddMinutes(_options.LifetimeMinutes);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
        ];

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AccessTokenDto(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
