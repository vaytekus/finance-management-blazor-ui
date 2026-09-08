using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;

public class GetAllOperationsQueryHandler(
    IOperationRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetAllOperationsQuery, PagedResult<OperationResponse>>
{
    public async Task<PagedResult<OperationResponse>> Handle(
        GetAllOperationsQuery request,
        CancellationToken ct)
    {
        var page = await repository.GetPagedForUserAsync(currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}
