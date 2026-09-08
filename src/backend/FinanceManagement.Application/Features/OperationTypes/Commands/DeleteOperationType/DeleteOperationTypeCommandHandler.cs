using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.DeleteOperationType;

public class DeleteOperationTypeCommandHandler(
    IOperationTypeRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteOperationTypeCommand>
{
    public async Task Handle(DeleteOperationTypeCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        var isUsed = await repository.IsUsedInOperationsAsync(entity.Id, ct);

        if (isUsed)
        {
            var toTypeId = await ResolveReplacementAsync(request, entity, ct);
            await repository.ReassignOperationsAsync(entity.Id, toTypeId, ct);
        }

        repository.Delete(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Guid> ResolveReplacementAsync(
        DeleteOperationTypeCommand request,
        OperationType deletedType,
        CancellationToken ct)
    {
        if (request.ReplaceWithId.HasValue)
        {
            var replacement = await repository.GetByIdForUserAsync(
                request.ReplaceWithId.Value, currentUser.Id, ct)
                .OrThrowAsync(request.ReplaceWithId.Value);

            if (replacement.Kind != deletedType.Kind)
            {
                throw new ValidationException($"Replacement type must be the same kind ({deletedType.Kind}).");
            }

            return replacement.Id;
        }

        if (!string.IsNullOrWhiteSpace(request.ReplaceWithName))
        {
            var exists = await repository.ExistsByNameForUserAsync(request.ReplaceWithName, currentUser.Id, ct: ct);

            if (exists)
            {
                throw new ConflictException($"Operation type '{request.ReplaceWithName}' already exists.");
            }

            var newType = new OperationType
            {
                Id = Guid.NewGuid(), Name = request.ReplaceWithName, Kind = deletedType.Kind, UserId = currentUser.Id
            };

            repository.Add(newType);
            await unitOfWork.SaveChangesAsync(ct);
            return newType.Id;
        }

        throw new ConflictException("This type is used in operations. Please provide a replacement type.");
    }
}
