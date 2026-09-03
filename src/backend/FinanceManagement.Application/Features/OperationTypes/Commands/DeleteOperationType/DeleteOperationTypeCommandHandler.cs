using FinanceManagement.Application.Common;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.DeleteOperationType;

public class DeleteOperationTypeCommandHandler : IRequestHandler<DeleteOperationTypeCommand>
{
    private readonly IOperationTypeRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOperationTypeCommandHandler(
        IOperationTypeRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteOperationTypeCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        var isUsed = await _repository.IsUsedInOperationsAsync(entity.Id, ct);

        if (isUsed)
        {
            var toTypeId = await ResolveReplacementAsync(request, entity, ct);
            await _repository.ReassignOperationsAsync(entity.Id, toTypeId, ct);
        }

        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Guid> ResolveReplacementAsync(
        DeleteOperationTypeCommand request,
        OperationType deletedType,
        CancellationToken ct)
    {
        if (request.ReplaceWithId.HasValue)
        {
            var replacement = await _repository.GetByIdForUserAsync(
                request.ReplaceWithId.Value, _currentUser.Id, ct)
                .OrThrowAsync(request.ReplaceWithId.Value);

            if (replacement.Kind != deletedType.Kind)
            {
                throw new ValidationException($"Replacement type must be the same kind ({deletedType.Kind}).");
            }

            return replacement.Id;
        }

        if (!string.IsNullOrWhiteSpace(request.ReplaceWithName))
        {
            var exists = await _repository.ExistsByNameForUserAsync(request.ReplaceWithName, _currentUser.Id, ct: ct);

            if (exists)
            {
                throw new ConflictException($"Operation type '{request.ReplaceWithName}' already exists.");
            }

            var newType = new OperationType
            {
                Id = Guid.NewGuid(), Name = request.ReplaceWithName, Kind = deletedType.Kind, UserId = _currentUser.Id
            };

            _repository.Add(newType);
            await _unitOfWork.SaveChangesAsync(ct);
            return newType.Id;
        }

        throw new ConflictException("This type is used in operations. Please provide a replacement type.");
    }
}
