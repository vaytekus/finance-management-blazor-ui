using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using FinanceManagement.Domain.Entities;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;

public class CreateOperationTypeCommandHandler(
    IOperationTypeRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOperationTypeCommand, OperationTypeResponse>
{
    public async Task<OperationTypeResponse> Handle(CreateOperationTypeCommand request, CancellationToken ct)
    {
        await repository.ExistsByNameForUserAsync(request.Name, currentUser.Id, excludeId: null, ct)
            .ThrowIfExistsAsync($"Operation type with name '{request.Name}' already exists.");

        var entity = new OperationType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Kind = request.Kind,
            UserId = currentUser.Id,
        };

        repository.Add(entity);
        await unitOfWork.SaveChangesAsync(ct);
        return entity.ToResponse();
    }
}
