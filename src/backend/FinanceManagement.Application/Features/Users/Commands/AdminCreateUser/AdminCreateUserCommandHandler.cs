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

public class AdminCreateUserCommandHandler : IRequestHandler<AdminCreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserProvisioningService _userProvisioningService;
    private readonly IUnitOfWork _unitOfWork;

    public AdminCreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserProvisioningService userProvisioningService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userProvisioningService = userProvisioningService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> Handle(AdminCreateUserCommand request, CancellationToken ct)
    {
        await _userRepository.UserNameExistsAsync(request.UserName, ct)
            .ThrowIfExistsAsync($"Username '{request.UserName}' is already taken.");

        await _userRepository.EmailExistsAsync(request.Email, ct)
            .ThrowIfExistsAsync($"Email '{request.Email}' is already registered.");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            RoleId = Enum.Parse<UserRole>(request.Role),
            CreatedAt = DateTime.UtcNow,
        };

        _userRepository.Add(user);
        _userProvisioningService.AddDefaultsFor(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return user.ToResponse();
    }
}
