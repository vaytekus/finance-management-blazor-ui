using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var stored = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null || stored.IsRevoked)
        {
            return;
        }

        stored.RevokedAt = DateTime.UtcNow;
        stored.ReasonRevoked = "Logout";
        await unitOfWork.SaveChangesAsync(ct);
    }
}
