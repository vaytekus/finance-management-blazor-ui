using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetOperationById;

public class GetOperationByIdQueryHandler : IRequestHandler<GetOperationByIdQuery, OperationResponse>
{
    private readonly IOperationRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetOperationByIdQueryHandler(IOperationRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<OperationResponse> Handle(GetOperationByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
