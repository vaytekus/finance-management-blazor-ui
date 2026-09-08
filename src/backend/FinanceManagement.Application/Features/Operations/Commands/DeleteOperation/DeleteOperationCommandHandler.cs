using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.DeleteOperation;

public class DeleteOperationCommandHandler(
    IOperationRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteOperationCommand>
{
    public async Task Handle(DeleteOperationCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        repository.SoftDelete(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
