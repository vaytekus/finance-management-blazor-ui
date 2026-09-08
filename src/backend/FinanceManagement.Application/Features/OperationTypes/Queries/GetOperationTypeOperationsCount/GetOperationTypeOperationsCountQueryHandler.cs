using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeOperationsCount;

public class GetOperationTypeOperationsCountQueryHandler(
    IOperationTypeRepository operationTypeRepository,
    ICurrentUser currentUser) : IRequestHandler<GetOperationTypeOperationsCountQuery, int>
{
    public async Task<int> Handle(GetOperationTypeOperationsCountQuery request, CancellationToken ct)
    {
        await operationTypeRepository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return await operationTypeRepository.CountOperationsAsync(request.Id, ct);
    }
}
