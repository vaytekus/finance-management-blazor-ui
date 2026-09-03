using FinanceManagement.Domain.Enums;
using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.UpdateOperationType;

public record UpdateOperationTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    OperationKind Kind) : IRequest;
