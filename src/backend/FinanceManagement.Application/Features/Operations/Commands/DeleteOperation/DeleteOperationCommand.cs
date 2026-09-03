using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.DeleteOperation;

public record DeleteOperationCommand(Guid Id) : IRequest;
