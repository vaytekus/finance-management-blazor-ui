using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;

public class UpdateOperationTypeCommandHandler : IRequestHandler<UpdateOperationTypeCommand>
{
    private readonly IOperationTypeRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOperationTypeCommandHandler(
        IOperationTypeRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateOperationTypeCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        await _repository.ExistsByNameForUserAsync(request.Name, _currentUser.Id, excludeId: request.Id, ct)
            .ThrowIfExistsAsync($"Operation type with name '{request.Name}' already exists.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Kind = request.Kind;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
