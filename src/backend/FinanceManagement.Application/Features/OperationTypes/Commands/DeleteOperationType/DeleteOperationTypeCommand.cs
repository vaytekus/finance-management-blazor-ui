using MediatR;

namespace FinanceManagement.Application.Features.OperationTypes.Commands.DeleteOperationType;

public record DeleteOperationTypeCommand(
    Guid Id,
    Guid? ReplaceWithId,
    string? ReplaceWithName) : IRequest;
