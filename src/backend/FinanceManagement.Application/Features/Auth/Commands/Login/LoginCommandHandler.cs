using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenIssuer tokenIssuer,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByUserNameAsync(request.UserName, ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var tokens = _tokenIssuer.Issue(user);
        await _unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = tokens,
            User = user.ToResponse()
        };
    }
}
