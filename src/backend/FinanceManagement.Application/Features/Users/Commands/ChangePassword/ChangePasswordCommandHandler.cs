using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangePasswordCommand>
{
    private const string _revokeReasonPasswordChange = "Password changed";

    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct).OrThrowAsync(request.UserId);

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Current password is incorrect.");
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(user.Id, ct);
        refreshTokenRepository.RevokeAll(activeTokens, _revokeReasonPasswordChange);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
