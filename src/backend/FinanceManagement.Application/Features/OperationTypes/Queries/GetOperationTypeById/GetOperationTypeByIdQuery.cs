using FinanceManagement.Contracts.OperationTypes;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Queries.GetOperationTypeById;

public record GetOperationTypeByIdQuery(Guid Id) : IRequest<OperationTypeResponse>;