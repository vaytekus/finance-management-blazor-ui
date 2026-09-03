using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetAllOperations;

public class GetAllOperationsQueryHandler : IRequestHandler<GetAllOperationsQuery, PagedResult<OperationResponse>>
{
    private readonly IOperationRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetAllOperationsQueryHandler(IOperationRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<OperationResponse>> Handle(
        GetAllOperationsQuery request, 
        CancellationToken ct)
    {
        var page = await _repository.GetPagedForUserAsync(_currentUser.Id, request, ct);

        return page.Map(x => x.ToResponse());
    }
}
