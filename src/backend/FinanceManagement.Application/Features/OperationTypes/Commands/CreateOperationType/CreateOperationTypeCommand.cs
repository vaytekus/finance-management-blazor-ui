using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.CreateOperationType;

public record CreateOperationTypeCommand(
    string Name,
    string? Description,
    OperationKind Kind) : IRequest<OperationTypeResponse>;
