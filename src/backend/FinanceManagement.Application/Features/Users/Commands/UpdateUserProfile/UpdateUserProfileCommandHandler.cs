using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.UserId, ct).OrThrowAsync(request.UserId);

        if (!string.Equals(user.UserName, request.UserName, StringComparison.Ordinal))
        {
            await repository.UserNameExistsAsync(request.UserName, ct)
                .ThrowIfExistsAsync($"Username '{request.UserName}' is already taken.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            await repository.EmailExistsAsync(request.Email, ct)
                .ThrowIfExistsAsync($"Email '{request.Email}' is already registered.");
        }

        user.UserName = request.UserName;
        user.Email = request.Email;

        await unitOfWork.SaveChangesAsync(ct);
    }
}
