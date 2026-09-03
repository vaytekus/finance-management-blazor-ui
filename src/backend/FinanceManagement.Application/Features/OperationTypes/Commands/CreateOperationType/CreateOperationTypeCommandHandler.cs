using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;

public class CreateOperationTypeCommandHandler : IRequestHandler<CreateOperationTypeCommand, OperationTypeResponse>
{
    private readonly IOperationTypeRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOperationTypeCommandHandler(
        IOperationTypeRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationTypeResponse> Handle(CreateOperationTypeCommand request, CancellationToken ct)
    {
        await _repository.ExistsByNameForUserAsync(request.Name, _currentUser.Id, excludeId: null, ct)
            .ThrowIfExistsAsync($"Operation type with name '{request.Name}' already exists.");

        var entity = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Kind = request.Kind,
            UserId = _currentUser.Id,
        };

        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity.ToResponse();
    }
}
