using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Domain.Common;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeUserRoleCommand>
{
    public async Task Handle(ChangeUserRoleCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct).OrThrowAsync(request.Id);
        var newRoleId = Enum.Parse<UserRole>(request.Role);

        if (user.RoleId == UserRole.Admin
            && newRoleId != UserRole.Admin
            && !await repository.AnyOtherAdminAsync(user.Id, ct))
        {
            throw new ValidationException("Cannot demote the last admin.");
        }

        user.RoleId = newRoleId;
        await unitOfWork.SaveChangesAsync(ct);
    }
}
