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

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IUserProvisioningService _userProvisioningService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenIssuer tokenIssuer,
        IUserProvisioningService userProvisioningService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
        _userProvisioningService = userProvisioningService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
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
            RoleId = UserRole.User,
            CreatedAt = DateTime.UtcNow,
        };

        _userRepository.Add(user);
        _userProvisioningService.AddDefaultsFor(user);

        var tokens = _tokenIssuer.Issue(user);
        await _unitOfWork.SaveChangesAsync(ct);
        return new AuthResult
        {
            Tokens = tokens,
            User = user.ToResponse()
        };
    }
}
