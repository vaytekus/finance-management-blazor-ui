using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeById;

public class GetOperationTypeByIdQueryHandler : IRequestHandler<GetOperationTypeByIdQuery, OperationTypeResponse>
{
    private readonly IOperationTypeRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetOperationTypeByIdQueryHandler(IOperationTypeRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<OperationTypeResponse> Handle(GetOperationTypeByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
