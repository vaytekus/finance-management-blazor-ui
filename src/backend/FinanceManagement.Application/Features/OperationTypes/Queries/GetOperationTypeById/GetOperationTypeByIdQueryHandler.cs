using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeById;

public class GetOperationTypeByIdQueryHandler(
    IOperationTypeRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetOperationTypeByIdQuery, OperationTypeResponse>
{
    public async Task<OperationTypeResponse> Handle(GetOperationTypeByIdQuery request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
