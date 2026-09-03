using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (request.Id == _currentUser.Id)
        {
            throw new ValidationException("You cannot delete your own account.");
        }

        var user = await _repository.GetByIdAsync(request.Id, ct).OrThrowAsync(request.Id);

        if (user.RoleId == UserRole.Admin && !await _repository.AnyOtherAdminAsync(user.Id, ct))
        {
            throw new ValidationException("Cannot delete the last admin.");
        }

        await _repository.DeleteOperationsAsync(user.Id, ct);
        _repository.Remove(user);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
