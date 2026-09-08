using FinanceManagement.Application.Common;
using FinanceManagement.Contracts.Operations;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Application.Mappings;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetOperationById;

public class GetOperationByIdQueryHandler(
    IOperationRepository repository,
    ICurrentUser currentUser) : IRequestHandler<GetOperationByIdQuery, OperationResponse>
{
    public async Task<OperationResponse> Handle(GetOperationByIdQuery request, CancellationToken ct)
    {
        var entity = await repository.GetByIdForUserAsync(request.Id, currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        return entity.ToResponse();
    }
}
