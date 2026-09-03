using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeOperationsCount;

public class GetOperationTypeOperationsCountQueryHandler : IRequestHandler<GetOperationTypeOperationsCountQuery, int>
{
    private readonly IOperationTypeRepository _operationTypeRepository;
    private readonly ICurrentUser _currentUser;

    public GetOperationTypeOperationsCountQueryHandler(
        IOperationTypeRepository operationTypeRepository,
        ICurrentUser currentUser)
    {
        _operationTypeRepository = operationTypeRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetOperationTypeOperationsCountQuery request, CancellationToken ct)
    {
        await _operationTypeRepository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return await _operationTypeRepository.CountOperationsAsync(request.Id, ct);
    }
}
