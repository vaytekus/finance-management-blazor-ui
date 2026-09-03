using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null || stored.IsRevoked)
        {
            return;
        }

        stored.RevokedAt = DateTime.UtcNow;
        stored.ReasonRevoked = "Logout";
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
