using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private const string _invalidRefreshTokenMessage = "Invalid refresh token.";

    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenIssuer tokenIssuer,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenIssuer = tokenIssuer;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null)
        {
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        if (stored.IsRevoked)
        {
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(stored.UserId, ct);
            _refreshTokenRepository.RevokeAll(activeTokens, "Reuse detected");
            await _unitOfWork.SaveChangesAsync(ct);
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        if (stored.IsExpired)
        {
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        var newTokens = _tokenIssuer.Issue(stored.User);

        stored.RevokedAt = DateTime.UtcNow;
        stored.ReasonRevoked = "Rotated";
        stored.ReplacedByToken = newTokens.RefreshToken;

        await _unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = newTokens,
            User = stored.User.ToResponse()
        };
    }
}
