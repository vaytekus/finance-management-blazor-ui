using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;

public class GetAllOperationTypesQueryHandler(
    IOperationTypeRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetAllOperationTypesQuery, PagedResult<OperationTypeResponse>>
{
    public async Task<PagedResult<OperationTypeResponse>> Handle(
        GetAllOperationTypesQuery request,
        CancellationToken ct)
    {
        var page = await repository.GetPagedForUserAsync(currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}