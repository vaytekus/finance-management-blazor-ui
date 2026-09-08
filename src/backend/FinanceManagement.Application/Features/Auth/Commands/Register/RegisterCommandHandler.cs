using FinanceManagement.Application.Common;
using FinanceManagement.Application.DTOs.Auth;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenIssuer tokenIssuer,
    IUserProvisioningService userProvisioningService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
    {
        await userRepository.UserNameExistsAsync(request.UserName, ct)
            .ThrowIfExistsAsync($"Username '{request.UserName}' is already taken.");

        await userRepository.EmailExistsAsync(request.Email, ct)
            .ThrowIfExistsAsync($"Email '{request.Email}' is already registered.");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            RoleId = UserRole.User,
            CreatedAt = DateTime.UtcNow,
        };

        userRepository.Add(user);
        userProvisioningService.AddDefaultsFor(user);

        var tokens = tokenIssuer.Issue(user);
        await unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = tokens,
            User = user.ToResponse()
        };
    }
}
