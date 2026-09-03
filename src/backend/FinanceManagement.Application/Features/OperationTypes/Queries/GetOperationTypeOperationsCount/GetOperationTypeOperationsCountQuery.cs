using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeOperationsCount;

public record GetOperationTypeOperationsCountQuery(Guid Id) : IRequest<int>;
