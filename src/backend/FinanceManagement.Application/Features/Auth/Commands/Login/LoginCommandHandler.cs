using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenIssuer tokenIssuer,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByUserNameAsync(request.UserName, ct);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var tokens = tokenIssuer.Issue(user);
        await unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = tokens,
            User = user.ToResponse()
        };
    }
}
