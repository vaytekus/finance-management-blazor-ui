using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Application.Options;
using Microsoft.Extensions.Options;

namespace FinanceManagement.Application.Services;

public class TokenIssuer(
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtSettings> jwtOptions) : ITokenIssuer
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public AuthTokens Issue(User user)
    {
        var (accessToken, accessExpiresAt) = jwtTokenGenerator.GenerateToken(user);

        var refreshTokenValue = refreshTokenGenerator.Generate();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);

        refreshTokenRepository.Add(new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt =  refreshExpiresAt,
        });

        return new AuthTokens
        {
            AccessToken = accessToken, AccessTokenExpiresAt = accessExpiresAt, RefreshToken = refreshTokenValue, RefreshTokenExpiresAt = refreshExpiresAt,
        };
    }
}
