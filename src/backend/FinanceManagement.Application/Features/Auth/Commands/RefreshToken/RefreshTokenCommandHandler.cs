using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenIssuer tokenIssuer,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private const string _invalidRefreshTokenMessage = "Invalid refresh token.";

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var stored = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null)
        {
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        if (stored.IsRevoked)
        {
            var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(stored.UserId, ct);
            refreshTokenRepository.RevokeAll(activeTokens, "Reuse detected");
            await unitOfWork.SaveChangesAsync(ct);
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        if (stored.IsExpired)
        {
            throw new UnauthorizedException(_invalidRefreshTokenMessage);
        }

        var newTokens = tokenIssuer.Issue(stored.User);

        stored.RevokedAt = DateTime.UtcNow;
        stored.ReasonRevoked = "Rotated";
        stored.ReplacedByToken = newTokens.RefreshToken;

        await unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = newTokens,
            User = stored.User.ToResponse()
        };
    }
}
