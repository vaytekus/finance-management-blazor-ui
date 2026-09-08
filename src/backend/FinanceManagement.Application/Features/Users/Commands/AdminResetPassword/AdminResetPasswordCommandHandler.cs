using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<AdminResetPasswordCommand>
{
    public async Task Handle(AdminResetPasswordCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct)
            .OrThrowAsync(request.UserId);

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(user.Id);
        refreshTokenRepository.RevokeAll(activeTokens, "Password reset by admin");

        await unitOfWork.SaveChangesAsync(ct);
    }
}
