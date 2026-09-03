using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Application.Options;
using Microsoft.Extensions.Options;

namespace FinanceManagement.Application.Services;

public class TokenIssuer : ITokenIssuer
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public TokenIssuer(
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings.Value;
    }
    
    public AuthTokens Issue(User user)
    {
        var (accessToken, accessExpiresAt) = _jwtTokenGenerator.GenerateToken(user);

        var refreshTokenValue = _refreshTokenGenerator.Generate();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);

        _refreshTokenRepository.Add(new RefreshToken
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
