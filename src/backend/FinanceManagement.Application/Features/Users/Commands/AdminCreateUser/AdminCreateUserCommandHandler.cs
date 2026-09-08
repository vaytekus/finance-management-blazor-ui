using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Contracts.Users;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.AdminCreateUser;

public class AdminCreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUserProvisioningService userProvisioningService,
    IUnitOfWork unitOfWork) : IRequestHandler<AdminCreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(AdminCreateUserCommand request, CancellationToken ct)
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
            RoleId = Enum.Parse<UserRole>(request.Role),
            CreatedAt = DateTime.UtcNow,
        };

        userRepository.Add(user);
        userProvisioningService.AddDefaultsFor(user);
        await unitOfWork.SaveChangesAsync(ct);

        return user.ToResponse();
    }
}
