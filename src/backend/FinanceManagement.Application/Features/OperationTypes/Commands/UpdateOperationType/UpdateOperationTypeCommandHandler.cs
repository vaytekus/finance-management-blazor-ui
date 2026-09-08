using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;

public class UpdateOperationTypeCommandHandler(
    IOperationTypeRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOperationTypeCommand>
{
    public async Task Handle(UpdateOperationTypeCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        await repository.ExistsByNameForUserAsync(request.Name, currentUser.Id, excludeId: request.Id, ct)
            .ThrowIfExistsAsync($"Operation type with name '{request.Name}' already exists.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Kind = request.Kind;

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
