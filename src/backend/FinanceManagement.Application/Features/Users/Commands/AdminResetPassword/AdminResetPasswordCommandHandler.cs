using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private IUnitOfWork _unitOfWork;

    public AdminResetPasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AdminResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            .OrThrowAsync(request.UserId);

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id);
        _refreshTokenRepository.RevokeAll(activeTokens, "Password reset by admin");

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
