using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    IUserRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (request.Id == currentUser.Id)
        {
            throw new ValidationException("You cannot delete your own account.");
        }

        var user = await repository.GetByIdAsync(request.Id, ct).OrThrowAsync(request.Id);

        if (user.RoleId == UserRole.Admin && !await repository.AnyOtherAdminAsync(user.Id, ct))
        {
            throw new ValidationException("Cannot delete the last admin.");
        }

        await repository.DeleteOperationsAsync(user.Id, ct);
        repository.Remove(user);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
