using FinanceManagement.Contracts.Operations;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Queries.GetOperationById;

public record GetOperationByIdQuery(Guid Id) : IRequest<OperationResponse>;
