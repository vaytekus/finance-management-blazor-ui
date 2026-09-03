using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct).OrThrowAsync(request.UserId);

        if (!string.Equals(user.UserName, request.UserName, StringComparison.Ordinal))
        {
            await _repository.UserNameExistsAsync(request.UserName, ct)
                .ThrowIfExistsAsync($"Username '{request.UserName}' is already taken.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            await _repository.EmailExistsAsync(request.Email, ct)
                .ThrowIfExistsAsync($"Email '{request.Email}' is already registered.");
        }

        user.UserName = request.UserName;
        user.Email = request.Email;

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
