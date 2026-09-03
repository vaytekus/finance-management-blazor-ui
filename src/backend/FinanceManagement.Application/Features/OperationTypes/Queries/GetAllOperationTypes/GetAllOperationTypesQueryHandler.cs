using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetAllOperationTypes;

public class GetAllOperationTypesQueryHandler : IRequestHandler<GetAllOperationTypesQuery, PagedResult<OperationTypeResponse>>
{
    private readonly IOperationTypeRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetAllOperationTypesQueryHandler(IOperationTypeRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<OperationTypeResponse>> Handle(
        GetAllOperationTypesQuery request, 
        CancellationToken ct)
    {
        var page = await _repository.GetPagedForUserAsync(_currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}